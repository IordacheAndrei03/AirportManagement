using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public async Task<int> CreateFlightAsync(FlightCreateDto flightCreateDto)
        {
            if (flightCreateDto.OriginIata.Equals(flightCreateDto.DestinationIata, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Origin and destination airports must be different.", nameof(flightCreateDto));
            }

            var airline = await _unitOfWork.AirlineRepository.GetByIataCodeAsync(flightCreateDto.AirlineIata);
            var originAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(flightCreateDto.OriginIata);
            var destinationAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(flightCreateDto.DestinationIata);
            var aircraft = await _unitOfWork.AircraftRepository.GetByTailNoAsync(flightCreateDto.DefaultAircraftTail);

            if (airline is null)
            {
                throw new ArgumentException($"Unknown airline IATA code '{flightCreateDto.AirlineIata}'.", nameof(flightCreateDto.AirlineIata));
            }

            if (originAirport is null)
            {
                throw new ArgumentException($"Unknown origin airport IATA code '{flightCreateDto.OriginIata}'.", nameof(flightCreateDto.OriginIata));
            }

            if (destinationAirport is null)
            {
                throw new ArgumentException($"Unknown destination airport IATA code '{flightCreateDto.DestinationIata}'.", nameof(flightCreateDto.DestinationIata));
            }

            if (aircraft is null)
            {
                throw new ArgumentException($"Unknown aircraft tail number '{flightCreateDto.DefaultAircraftTail}'.", nameof(flightCreateDto.DefaultAircraftTail));
            }

            var hasDuplicate = await _unitOfWork.FlightRepository.ExistsDuplicateRouteAsync(
                airline.Id,
                flightCreateDto.FlightNumber,
                originAirport.Id,
                destinationAirport.Id,
                excludeFlightId: null);

            if (hasDuplicate)
            {
                throw new ArgumentException("A flight with the same airline, number and route already exists.");
            }

            var newFlight = new Flight
            {
                AirlineId = airline.Id,
                FlightNumber = flightCreateDto.FlightNumber,
                OriginAirport = originAirport.Id,
                DestinationAirport = destinationAirport.Id,
                DefaultAircraftId = aircraft.Id,
                IsActive = flightCreateDto.IsActive
            };

            var flight = _mapper.Map<Flight>(newFlight);

            if (flight.OriginAirport == flight.DestinationAirport)
            {
                throw new InvalidOperationException(
                    $"OriginAirport and DestinationAirport are equal (Id={flight.OriginAirport}).");
            }

            await _unitOfWork.FlightRepository.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();
            return flight.Id;
        }

        public async Task UpdateAsync(int id, FlightCreateDto flightUpdateDto)
        {
            if (flightUpdateDto.OriginIata.Equals(flightUpdateDto.DestinationIata, StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Origin and destination airports must be different.", nameof(flightUpdateDto));
            }

            var flightRepo = _unitOfWork.FlightRepository;
            var flight = await flightRepo.GetByIdAsync(id);

            if (flight is null)
            {
                throw new KeyNotFoundException($"Flight with id {id} not found.");
            }

            var airlineRepo = _unitOfWork.AirlineRepository;
            var airportRepo = _unitOfWork.AirportRepository;
            var aircraftRepo = _unitOfWork.AircraftRepository;

            var airline = await airlineRepo.GetByIataCodeAsync(flightUpdateDto.AirlineIata)
                ?? throw new ArgumentException($"Unknown airline IATA code '{flightUpdateDto.AirlineIata}'.",
                    nameof(flightUpdateDto.AirlineIata));

            var originAirport = await airportRepo.GetByIataCodeAsync(flightUpdateDto.OriginIata)
                ?? throw new ArgumentException($"Unknown origin airport IATA code '{flightUpdateDto.OriginIata}'.",
                    nameof(flightUpdateDto.OriginIata));

            var destinationAirport = await airportRepo.GetByIataCodeAsync(flightUpdateDto.DestinationIata)
                ?? throw new ArgumentException($"Unknown destination airport IATA code '{flightUpdateDto.DestinationIata}'.",
                    nameof(flightUpdateDto.DestinationIata));

            if (originAirport.Id == destinationAirport.Id)
            {
                throw new ArgumentException("Origin and destination airports must be different.", nameof(flightUpdateDto));
            }

            var aircraft = await aircraftRepo.GetByTailNoAsync(flightUpdateDto.DefaultAircraftTail)
                ?? throw new ArgumentException($"Unknown aircraft tail number '{flightUpdateDto.DefaultAircraftTail}'.",
                    nameof(flightUpdateDto.DefaultAircraftTail));

            var hasDuplicate = await _unitOfWork.FlightRepository.ExistsDuplicateRouteAsync(
                airline.Id,
                flightUpdateDto.FlightNumber,
                originAirport.Id,
                destinationAirport.Id,
                excludeFlightId: id);

            if (hasDuplicate)
            {
                throw new ArgumentException("A flight with the same airline, number and route already exists.");
            }

            flight.AirlineId = airline.Id;
            flight.FlightNumber = flightUpdateDto.FlightNumber;
            flight.OriginAirport = originAirport.Id;
            flight.DestinationAirport = destinationAirport.Id;
            flight.DefaultAircraftId = aircraft.Id;
            flight.IsActive = flightUpdateDto.IsActive;

            if (flight.OriginAirport == flight.DestinationAirport)
            {
                throw new InvalidOperationException("OriginAirport and DestinationAirport ended up equal.");
            }

            flightRepo.Update(flight);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var flightRepo = _unitOfWork.FlightRepository;
            var flight = await flightRepo.GetByIdAsync(id);

            if (flight is null)
            {
                throw new KeyNotFoundException($"Flight with id {id} not found.");
            }

            var flightScheduleRepo = _unitOfWork.FlightScheduleRepository;

            var hasSchedules = await _unitOfWork.FlightScheduleRepository.AnyByFlightIdAsync(id);

            if (hasSchedules)
            {
                throw new InvalidOperationException(
                    "Flight cannot be deleted because it has associated schedules.");
            }

            await flightRepo.DeleteByIdAsync(id);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
