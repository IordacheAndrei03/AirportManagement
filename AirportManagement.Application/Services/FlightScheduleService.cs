using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Exceptions;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AirportManagement.Application.Services
{

    public class FlightScheduleService : IFlightScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FlightScheduleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultObject<FlightScheduleDetailsDto>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.FlightScheduleRepository.GetByIdWithDetailsAsync(id);

            if (entity is null)
            {
                throw new NotFoundException("FlightSchedule",id);
            }

            var flightScheduleDto = _mapper.Map<FlightScheduleDetailsDto>(entity);

            return ResultObject<FlightScheduleDetailsDto>.Success(flightScheduleDto);
        }

        public async Task<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStatsAsync(int days)
        {
            if (days <= 0)
            {
                throw new BadRequestException("Number of days must be positive");
            }

            var rows = await _unitOfWork.FlightScheduleRepository.GetUpcomingStatsAsync(days);

            if (rows == null || rows.Count == 0)
            {
                return ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.NotFound(
                    $"No upcoming flights found for the {days} days.");
            }

            var result = _mapper.Map<IReadOnlyList<UpcomingSchedulesDto>>(rows);

            return ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Success(result);
        }

        public async Task<int> CreateAsync(FlightScheduleCreateDto dto)
        {
            if (dto.ScheduledArrivalUtc <= dto.ScheduledDepartureUtc)
            {
                throw new BadRequestException("Arrival must be after departure.");
            }

            var hasOverlap = await _unitOfWork.FlightScheduleRepository.HasGateOverlapAsync(
                dto.GateId,
                dto.ScheduledDepartureUtc,
                dto.ScheduledArrivalUtc,
                ignoreScheduleId: null);

            if (hasOverlap)
            {
                throw new ConflictException("Gate is already used for this time interval.");
            }

            var plannedStatusId = await _unitOfWork.FlightStatusRepository.GetStatusIdByNameAsync(FlightScheduleStatus.Scheduled);

            if (plannedStatusId == 0)
            {
                throw new NotFoundException("Flight status", plannedStatusId);
            }

            var entity = new FlightSchedule
            {
                FlightId = dto.FlightId,
                ScheduledDepartureUtc = dto.ScheduledDepartureUtc,
                ScheduleArrivalUtc = dto.ScheduledArrivalUtc,
                GateId = dto.GateId,
                AssignedAircraftId = dto.AssignedAircraftId,
                FlightStatusId = plannedStatusId
            };

            await _unitOfWork.FlightScheduleRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<ScheduleImportResultDto> ImportAsync(
         IFormFile file,
         CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty.");

            if (file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("File is too large. Max 2 MB.");

            List<ScheduleImportRowDto>? rows;
            using (var stream = file.OpenReadStream())
            {
                rows = await JsonSerializer.DeserializeAsync<List<ScheduleImportRowDto>>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true },
                    cancellationToken);
            }

            if (rows == null || rows.Count == 0)
                throw new ArgumentException("File does not contain any schedules.");

            if (rows.Count > 1000)
                throw new ArgumentException("File contains more than 1000 rows (limit 1000).");

            var result = new ScheduleImportResultDto { Total = rows.Count };

            var plannedStatusId = await _unitOfWork.FlightStatusRepository.GetStatusIdByNameAsync(FlightScheduleStatus.Scheduled, cancellationToken);

            var airlineRepo = _unitOfWork.AirlineRepository;
            var airportRepo = _unitOfWork.AirportRepository;
            var aircraftRepo = _unitOfWork.AircraftRepository;
            var flightRepo = _unitOfWork.FlightRepository;

            var rowIndex = 0;
            foreach (var row in rows)
            {
                rowIndex++;

                try
                {
                    if (row.ScheduledArrivalUtc <= row.ScheduledDepartureUtc)
                        throw new Exception("Arrival must be after departure.");

                    var airline = await airlineRepo.GetByIataCodeAsync(row.AirlineIata)
                        ?? throw new BadRequestException($"Unknown airline IATA code '{row.AirlineIata}.");


                    var originAirport = await airportRepo.GetByIataCodeAsync(row.OriginIata)
                        ?? throw new BadRequestException($"Unknown origin airport IATA code '{row.OriginIata}'.");

                    var destAirport = await airportRepo.GetByIataCodeAsync(row.DestinationIata)
                ?? throw new BadRequestException($"Unknown destination airport IATA code '{row.DestinationIata}'.");

                    var aircraft = await aircraftRepo.GetByTailNoAsync(row.AssignedAircraftTail)
                ?? throw new BadRequestException($"Unknown aircraft tail number '{row.AssignedAircraftTail}'.");

                    var flight = await _unitOfWork.FlightRepository
                        .FindByAirlineNumberAndRouteAsync(airline.Id, row.FlightNumber, originAirport.Id, destAirport.Id, cancellationToken);

                    if (flight == null)
                    {
                        var newFlight = new Flight
                        {
                            AirlineId = airline.Id,
                            FlightNumber = row.FlightNumber,
                            OriginAirport = originAirport.Id,
                            DestinationAirport = destAirport.Id,
                            DefaultAircraftId = aircraft.Id,
                            IsActive = true
                        };

                        await _unitOfWork.FlightRepository.AddAsync(newFlight);
                        await _unitOfWork.SaveChangesAsync(); 
                        flight = newFlight;
                    }

                    var gate = await _unitOfWork.GateRepository
                        .GetByAirportIdAndCodeAsync(originAirport.Id, row.GateCode, cancellationToken);

                    var hasOverlap = await _unitOfWork.FlightScheduleRepository.HasGateOverlapAsync(
                        gate.Id,
                        row.ScheduledDepartureUtc,
                        row.ScheduledArrivalUtc,
                        ignoreScheduleId: null);

                    if (hasOverlap)
                    {
                        throw new Exception($"Gate overlap at {row.OriginIata}:{row.GateCode} {row.ScheduledDepartureUtc:O}–{row.ScheduledArrivalUtc:O}");
                    }

                    var existing = await _unitOfWork.FlightScheduleRepository
                        .FindByFlightAndDepartureAsync(flight.Id, row.ScheduledDepartureUtc, cancellationToken);

                    if (existing == null)
                    {
                        var schedule = new FlightSchedule
                        {
                            FlightId = flight.Id,
                            ScheduledDepartureUtc = row.ScheduledDepartureUtc,
                            ScheduleArrivalUtc = row.ScheduledArrivalUtc,
                            GateId = gate.Id,
                            AssignedAircraftId = aircraft.Id,
                            FlightStatusId = plannedStatusId
                        };

                        await _unitOfWork.FlightScheduleRepository.AddAsync(schedule); 
                        await _unitOfWork.SaveChangesAsync();
                        result.Created++;
                    }
                    else
                    {
                        existing.ScheduleArrivalUtc = row.ScheduledArrivalUtc;
                        existing.GateId = gate.Id;
                        existing.AssignedAircraftId = aircraft.Id;
                        existing.FlightStatusId = plannedStatusId;

                        _unitOfWork.FlightScheduleRepository.Update(existing); 
                        await _unitOfWork.SaveChangesAsync();
                        result.Updated++;
                    }
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ScheduleImportErrorDto { Row = rowIndex, Message = ex.Message });
                }
            }

            return result;
        }
    }
}

