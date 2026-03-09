using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces;
using AirportManagement.Application.Results;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace AirportManagement.Application.Services
{
    public class FlightScheduleService : IFlightScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IScheduleImportParser _importParser;
        private readonly IScheduleImportRowProcessor _rowProcessor;

        public FlightScheduleService(IUnitOfWork unitOfWork, IMapper mapper, IScheduleImportParser importParser, IScheduleImportRowProcessor rowProcessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _importParser = importParser;
            _rowProcessor = rowProcessor;
        }

        public async Task<ResultObject<FlightScheduleDetailsDto>> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.FlightScheduleRepository.GetByIdWithDetailsAsync(id);

            if (entity is null)
            {
                return ResultObject<FlightScheduleDetailsDto>.NotFound($"FlightSchedule with Id={id} not found.");
            }

            var flightScheduleDto = _mapper.Map<FlightScheduleDetailsDto>(entity);

            return ResultObject<FlightScheduleDetailsDto>.Success(flightScheduleDto);
        }

        public async Task<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStatsAsync(int days)
        {
            if (days <= 0)
            {
                return ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Invalid("Number of days must be positive");
            }

            var rows = await _unitOfWork.FlightScheduleRepository.GetUpcomingStatsAsync(days);
            if (rows == null || rows.Count == 0)
            {
                return ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.NotFound($"No upcoming flights found for the {days} days.");
            }

            return ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Success(rows);
        }

        public async Task<ResultObject<int>> CreateAsync(FlightScheduleCreateDto dto)
        {
            if (dto.ScheduledArrivalUtc <= dto.ScheduledDepartureUtc)
            {
                return ResultObject<int>.Invalid("Arrival must be after departure.");
            }

            var hasOverlap = await _unitOfWork.FlightScheduleRepository.HasGateOverlapAsync(
                dto.GateId,
                dto.ScheduledDepartureUtc);

            if (hasOverlap)
            {
                return ResultObject<int>.Conflict("Gate is already used for this time interval.");
            }

            var plannedStatusId = await _unitOfWork.FlightStatusRepository.GetStatusIdByNameAsync(FlightScheduleStatus.Scheduled);

            if (plannedStatusId == 0)
            {
                return ResultObject<int>.NotFound("Flight status 'Scheduled' not found.");
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

            return ResultObject<int>.Success(entity.Id);
        }

        public async Task<ResultObject<ScheduleImportResultDto>> ImportAsync(IFormFile file)
        {
            var rowsResult = await _importParser.ParseAsync(file);
            if (!rowsResult.IsSuccess)
            {
                return ResultObject<ScheduleImportResultDto>.Invalid(rowsResult.Error!);
            }

            var rows = rowsResult.Value!;
            var result = new ScheduleImportResultDto { Total = rows.Count };

            var plannedStatusId = await _unitOfWork.FlightStatusRepository.GetStatusIdByNameAsync(FlightScheduleStatus.Scheduled);

            var rowIndex = 0;

            foreach (var row in rows)
            {
                rowIndex++;

                try
                {
                    await _rowProcessor.ProcessRowAsync(row, plannedStatusId, result);
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ScheduleImportErrorDto { Row = rowIndex, Message = ex.Message });
                }
            }

            return ResultObject<ScheduleImportResultDto>.Success(result);
        }
    }
}

