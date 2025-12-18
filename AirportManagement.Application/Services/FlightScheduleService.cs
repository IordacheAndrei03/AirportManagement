using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Exceptions;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
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
    }
}

