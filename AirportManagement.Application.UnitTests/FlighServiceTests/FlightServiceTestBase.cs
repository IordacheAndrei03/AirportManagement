using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Services;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Moq;

public abstract class FlightServiceTestBase
{
    protected readonly Mock<IUnitOfWork> _unitOfWork;
    protected readonly Mock<IMapper> _mapper;
    protected readonly Mock<IFlightRepository> _flightRepository;
    protected readonly Mock<IFlightScheduleRepository> _flightScheduleRepository;
    protected readonly Mock<IAirlineRepository> _airlineRepository;
    protected readonly Mock<IAirportRepository> _airportRepository;
    protected readonly Mock<IAircraftRepository> _aircraftRepository;
    protected readonly FlightService _flightService;
    protected Flight? _capturedAddedFlight;
    protected int? _capturedDeletedFlightId;

    protected FlightServiceTestBase()
    {
        _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Loose);
        _mapper = new Mock<IMapper>(MockBehavior.Loose);

        _flightRepository = new Mock<IFlightRepository>(MockBehavior.Loose);
        _flightScheduleRepository = new Mock<IFlightScheduleRepository>(MockBehavior.Loose);
        _airlineRepository = new Mock<IAirlineRepository>(MockBehavior.Loose);
        _airportRepository = new Mock<IAirportRepository>(MockBehavior.Loose);
        _aircraftRepository = new Mock<IAircraftRepository>(MockBehavior.Loose);

        _unitOfWork.SetupGet(x => x.FlightRepository).Returns(_flightRepository.Object);
        _unitOfWork.SetupGet(x => x.FlightScheduleRepository).Returns(_flightScheduleRepository.Object);
        _unitOfWork.SetupGet(x => x.AirlineRepository).Returns(_airlineRepository.Object);
        _unitOfWork.SetupGet(x => x.AirportRepository).Returns(_airportRepository.Object);
        _unitOfWork.SetupGet(x => x.AircraftRepository).Returns(_aircraftRepository.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        _flightService = new FlightService(_unitOfWork.Object, _mapper.Object);
    }

    protected FlightCreateDto CreateValidFlightCreateDto()
    {
        return new FlightCreateDto
        {
            AirlineIata = "AA",            
            FlightNumber = "100",         
            OriginIata = "OTP",          
            DestinationIata = "LHR",     
            DefaultAircraftTail = "YR-ABC",
            IsActive = true
        };
    }

    protected DateOnly CreateValidDepartureDate()
    {
        return new DateOnly(2026, 1, 9);
    }
    protected Flight CreateExistingFlight(int id = 10)
    {
        return new Flight
        {
            Id = id,
            AirlineId = 1,
            FlightNumber = "OLD",
            OriginAirport = 10,
            DestinationAirport = 11,
            DefaultAircraftId = 100,
            IsActive = true
        };
    }

    protected void CaptureAddFlight(int forcedId)
    {
        _capturedAddedFlight = null;

        _flightRepository
            .Setup(x => x.AddAsync(It.IsAny<Flight>()))
            .Callback<Flight>(f =>
            {
                f.Id = forcedId;
                _capturedAddedFlight = f;
            })
            .Returns(Task.CompletedTask);
    }

    protected void CaptureDeleteFlightId()
    {
        _capturedDeletedFlightId = null;

        _flightRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .Callback<int>(id => _capturedDeletedFlightId = id)
            .ReturnsAsync(true); 
    }

    protected void ArrangeAllLookupsValid(FlightCreateDto dto, int airlineId = 1, int originId = 10, int destinationId = 11, int aircraftId = 100)
    {
        _airlineRepository
            .Setup(x => x.GetByIataCodeAsync(dto.AirlineIata))
            .ReturnsAsync(new Airline { Id = airlineId });

        _airportRepository
            .Setup(x => x.GetByIataCodeAsync(dto.OriginIata))
            .ReturnsAsync(new Airport { Id = originId });

        _airportRepository
            .Setup(x => x.GetByIataCodeAsync(dto.DestinationIata))
            .ReturnsAsync(new Airport { Id = destinationId });

        _aircraftRepository
            .Setup(x => x.GetByTailNoAsync(dto.DefaultAircraftTail))
            .ReturnsAsync(new Aircraft { Id = aircraftId });
    }
}