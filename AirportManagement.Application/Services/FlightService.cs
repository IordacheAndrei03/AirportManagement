using AirportManagement.Application.Dtos;
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
    public class FlightService : IFlightService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private const int DefaultPage = 1;
        private const int DefaultPageSize = 20;
        private const int MaxPageSize = 100;

        public FlightService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ResultObject<FlightDetailsDto>> GetByIdAsync(int id)
        {
            var flight = await _unitOfWork.FlightRepository.GetByIdWithDetailsAsync(id);
            var flightDto = _mapper.Map<FlightDetailsDto>(flight);

            return ResultObject<FlightDetailsDto>.Success(flightDto);
        }

        public async Task<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDateAsync(
             string originIata,
             string destinationIata,
             DateOnly departureDate,
             int page,
             int pageSize)
        {
            if (string.IsNullOrWhiteSpace(originIata) ||
                string.IsNullOrWhiteSpace(destinationIata))
            {
                return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Invalid(
                    "Origin and destination are required.");
            }

            if (originIata.Equals(destinationIata, StringComparison.OrdinalIgnoreCase))
            {
                return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Invalid(
                    "Origin and destination airports must be different.");
            }

            if (page <= 0) page = DefaultPage;
            if (pageSize <= 0) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            var departureDateUtc = departureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var schedules = await _unitOfWork.FlightScheduleRepository
                .SearchUpcomingByRouteAndDateAsync(
                    originIata,
                    destinationIata,
                    departureDateUtc,
                    page,
                    pageSize);

            var dtoList = _mapper.Map<IReadOnlyList<FlightSearchScheduleDto>>(schedules);

            return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Success(dtoList);
        }
    }
}
