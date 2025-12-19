using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Exceptions;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}

