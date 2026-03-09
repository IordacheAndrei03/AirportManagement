using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Results;
using AirportManagement.Domain.Entities;
using AutoMapper;

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

            if (flight is null)
            {
                return ResultObject<FlightDetailsDto>.NotFound($"Flight with Id={id} not found.");
            }

            var flightDto = _mapper.Map<FlightDetailsDto>(flight);

            return ResultObject<FlightDetailsDto>.Success(flightDto);
        }

        public async Task<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDateAsync(string originIata, string destinationIata, DateOnly departureDate, int page, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(originIata) || string.IsNullOrWhiteSpace(destinationIata))
            {
                return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Invalid("Origin and destination are required.");
            }

            if (originIata.Equals(destinationIata, StringComparison.OrdinalIgnoreCase))
            {
                return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Invalid("Origin and destination airports must be different.");
            }

            if (page <= 0) page = DefaultPage;
            if (pageSize <= 0) pageSize = DefaultPageSize;
            if (pageSize > MaxPageSize) pageSize = MaxPageSize;

            var departureDateUtc = departureDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var schedules = await _unitOfWork.FlightScheduleRepository.SearchUpcomingByRouteAndDateAsync(originIata, destinationIata, departureDateUtc, page, pageSize);

            var dtoList = _mapper.Map<IReadOnlyList<FlightSearchScheduleDto>>(schedules);

            return ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Success(dtoList);
        }

        public async Task<ResultObject<int>> CreateFlightAsync(FlightCreateDto flightCreateDto)
        {
            if (flightCreateDto.OriginIata.Equals(flightCreateDto.DestinationIata, StringComparison.OrdinalIgnoreCase))
            {
                return ResultObject<int>.Invalid("Origin and destination airports must be different.");
            }

            var airline = await _unitOfWork.AirlineRepository.GetByIataCodeAsync(flightCreateDto.AirlineIata);
            var originAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(flightCreateDto.OriginIata);
            var destinationAirport = await _unitOfWork.AirportRepository.GetByIataCodeAsync(flightCreateDto.DestinationIata);
            var aircraft = await _unitOfWork.AircraftRepository.GetByTailNoAsync(flightCreateDto.DefaultAircraftTail);

            if (airline is null)
            {
                return ResultObject<int>.Invalid($"Unknown airline IATA code '{flightCreateDto.AirlineIata}'.");
            }

            if (originAirport is null)
            {
                return ResultObject<int>.Invalid($"Unknown origin airport IATA code '{flightCreateDto.OriginIata}'.");
            }

            if (destinationAirport is null)
            {
                return ResultObject<int>.Invalid($"Unknown destination airport IATA code '{flightCreateDto.DestinationIata}'.");
            }

            if (aircraft is null)
            {
                return ResultObject<int>.Invalid($"Unknown aircraft tail number '{flightCreateDto.DefaultAircraftTail}'.");
            }

            var hasDuplicate = await _unitOfWork.FlightRepository.ExistsDuplicateRouteAsync(
                airline.Id,
                flightCreateDto.FlightNumber,
                originAirport.Id,
                destinationAirport.Id,
                excludeFlightId: null);

            if (hasDuplicate)
            {
                return ResultObject<int>.Conflict("A flight with the same airline, number and route already exists.");
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
                return ResultObject<int>.Conflict($"OriginAirport and DestinationAirport are equal (Id={flight.OriginAirport}).");
            }

            await _unitOfWork.FlightRepository.AddAsync(flight);
            await _unitOfWork.SaveChangesAsync();

            return ResultObject<int>.Success(flight.Id);
        }

        public async Task<Result> UpdateAsync(int id, FlightCreateDto flightUpdateDto)
        {
            if (flightUpdateDto.OriginIata.Equals(flightUpdateDto.DestinationIata, StringComparison.OrdinalIgnoreCase))
            {
                return Result.Invalid("Origin and destination airports must be different.");
            }

            var flightRepo = _unitOfWork.FlightRepository;
            var flight = await flightRepo.GetByIdAsync(id);

            if (flight is null)
            {
                return Result.NotFound($"Flight with Id={id} not found.");
            }

            var airlineRepo = _unitOfWork.AirlineRepository;
            var airportRepo = _unitOfWork.AirportRepository;
            var aircraftRepo = _unitOfWork.AircraftRepository;

            var airline = await airlineRepo.GetByIataCodeAsync(flightUpdateDto.AirlineIata);
            if (airline is null)
            {
                return Result.Invalid($"Unknown airline IATA code '{flightUpdateDto.AirlineIata}'.");
            }

            var originAirport = await airportRepo.GetByIataCodeAsync(flightUpdateDto.OriginIata);
            if (originAirport is null)
            {
                return Result.Invalid($"Unknown origin airport IATA code '{flightUpdateDto.OriginIata}'.");
            }

            var destinationAirport = await airportRepo.GetByIataCodeAsync(flightUpdateDto.DestinationIata);
            if (destinationAirport is null)
            {
                return Result.Invalid($"Unknown destination airport IATA code '{flightUpdateDto.DestinationIata}'."); ;
            }

            if (originAirport.Id == destinationAirport.Id)
            {
                return Result.Invalid("Origin and destination airports must be different.");
            }

            var aircraft = await aircraftRepo.GetByTailNoAsync(flightUpdateDto.DefaultAircraftTail);
            if (aircraft is null)
            {
                return Result.Invalid($"Unknown aircraft tail number '{flightUpdateDto.DefaultAircraftTail}'.");
            }

            var hasDuplicate = await _unitOfWork.FlightRepository.ExistsDuplicateRouteAsync(
                airline.Id,
                flightUpdateDto.FlightNumber,
                originAirport.Id,
                destinationAirport.Id,
                excludeFlightId: id);

            if (hasDuplicate)
            {
                return Result.Conflict("A flight with the same airline, number and route already exists.");
            }

            flight.AirlineId = airline.Id;
            flight.FlightNumber = flightUpdateDto.FlightNumber;
            flight.OriginAirport = originAirport.Id;
            flight.DestinationAirport = destinationAirport.Id;
            flight.DefaultAircraftId = aircraft.Id;
            flight.IsActive = flightUpdateDto.IsActive;

            if (flight.OriginAirport == flight.DestinationAirport)
            {
                return Result.Conflict("OriginAirport and DestinationAirport ended up equal.");
            }

            flightRepo.Update(flight);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var flightRepo = _unitOfWork.FlightRepository;
            var flight = await flightRepo.GetByIdAsync(id);

            if (flight is null)
            {
                return Result.NotFound($"Flight with Id={id} not found.");
            }

            var flightScheduleRepo = _unitOfWork.FlightScheduleRepository;

            var hasSchedules = await _unitOfWork.FlightScheduleRepository.AnyByFlightIdAsync(id);

            if (hasSchedules)
            {
                return Result.Conflict("Flight cannot be deleted because it has associated schedules.");
            }

            await flightRepo.DeleteByIdAsync(id);

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
    }
}
