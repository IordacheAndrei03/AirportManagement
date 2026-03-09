using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces.ImportInterfaces;
using AirportManagement.Application.Services;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Moq;

public abstract class FlightScheduleServiceTestBase
{
    protected readonly Mock<IUnitOfWork> _unitOfWork;
    protected readonly Mock<IMapper> _mapper;
    protected readonly Mock<IScheduleImportParser> _scheduleImportParser;
    protected readonly Mock<IScheduleImportRowProcessor> _scheduleImportRowProcessor;
    protected readonly Mock<IFlightScheduleRepository> _flightScheduleRepository;
    protected readonly Mock<IFlightStatusRepository> _flightStatusRepository;
    protected readonly FlightScheduleService _flightScheduleService;
    protected FlightSchedule? _capturedFlightSchedule;

    protected FlightScheduleServiceTestBase()
    {
        _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Loose);
        _mapper = new Mock<IMapper>(MockBehavior.Loose);
        _scheduleImportParser = new Mock<IScheduleImportParser>(MockBehavior.Loose);
        _scheduleImportRowProcessor = new Mock<IScheduleImportRowProcessor>(MockBehavior.Loose);

        _flightScheduleRepository = new Mock<IFlightScheduleRepository>(MockBehavior.Loose);
        _flightStatusRepository = new Mock<IFlightStatusRepository>(MockBehavior.Loose);

        _unitOfWork.SetupGet(x => x.FlightScheduleRepository).Returns(_flightScheduleRepository.Object);
        _unitOfWork.SetupGet(x => x.FlightStatusRepository).Returns(_flightStatusRepository.Object);
        _unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        _flightScheduleService = new FlightScheduleService(
            _unitOfWork.Object,
            _mapper.Object,
            _scheduleImportParser.Object,
            _scheduleImportRowProcessor.Object);
    }

    protected FlightScheduleCreateDto CreateValidCreateDto()
    {
        return new FlightScheduleCreateDto
        {
            FlightId = 100,
            GateId = 5,
            AssignedAircraftId = 77,
            ScheduledDepartureUtc = new DateTime(2026, 1, 9, 10, 0, 0, DateTimeKind.Utc),
            ScheduledArrivalUtc = new DateTime(2026, 1, 9, 11, 0, 0, DateTimeKind.Utc),
        };
    }

    protected void ArrangeNoGateOverlap(FlightScheduleCreateDto dto)
    {
        _flightScheduleRepository
            .Setup(x => x.HasGateOverlapAsync(dto.GateId, dto.ScheduledDepartureUtc))
            .ReturnsAsync(false);
    }

    protected void ArrangeGateOverlap(FlightScheduleCreateDto dto)
    {
        _flightScheduleRepository
            .Setup(x => x.HasGateOverlapAsync(dto.GateId, dto.ScheduledDepartureUtc))
            .ReturnsAsync(true);
    }

    protected void ArrangeScheduledStatusId(int statusId)
    {
        _flightStatusRepository
            .Setup(x => x.GetStatusIdByNameAsync(FlightScheduleStatus.Scheduled))
            .ReturnsAsync(statusId);
    }

    protected void ArrangeCaptureAddAsync(int forcedId)
    {
        _capturedFlightSchedule = null;

        _flightScheduleRepository
            .Setup(x => x.AddAsync(It.IsAny<FlightSchedule>()))
            .Callback<FlightSchedule>(entity =>
            {
                entity.Id = forcedId;
                _capturedFlightSchedule = entity;
            })
            .Returns(Task.CompletedTask);
    }
}