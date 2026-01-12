using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Services;
using AirportManagement.Domain.Entities;
using AutoMapper;
using Moq;

public abstract class TicketServiceTestBase
{
    protected readonly Mock<IUnitOfWork> _unitOfWork;
    protected readonly Mock<ICurrentUserService> _currentUserService;
    protected readonly Mock<IMapper> _mapper;

    protected readonly Mock<IBookingRepository> _bookingRepository;
    protected readonly Mock<IFlightScheduleRepository> _flightScheduleRepository;
    protected readonly Mock<ITicketRepository> _ticketRepository;

    protected readonly TicketService _ticketService;

    protected Ticket? _capturedAddedTicket;
    protected int? _capturedIncrementBookingId;
    protected int? _capturedDeletedTicketId;

    protected TicketServiceTestBase()
    {
        _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Loose);
        _currentUserService = new Mock<ICurrentUserService>(MockBehavior.Loose);
        _mapper = new Mock<IMapper>(MockBehavior.Loose);

        _bookingRepository = new Mock<IBookingRepository>(MockBehavior.Loose);
        _flightScheduleRepository = new Mock<IFlightScheduleRepository>(MockBehavior.Loose);
        _ticketRepository = new Mock<ITicketRepository>(MockBehavior.Loose);

        _unitOfWork.SetupGet(x => x.BookingRepository).Returns(_bookingRepository.Object);
        _unitOfWork.SetupGet(x => x.FlightScheduleRepository).Returns(_flightScheduleRepository.Object);
        _unitOfWork.SetupGet(x => x.TicketRepository).Returns(_ticketRepository.Object);

        _unitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

        _ticketService = new TicketService(_unitOfWork.Object, _currentUserService.Object, _mapper.Object);
    }

    protected TicketCreateRequestDto CreateValidTicketCreateRequestDto()
    {
        return new TicketCreateRequestDto
        {
            BookingId = 10,
            FlightScheduleId = 20,
            FareClass = "Economy",
            BasePrice = 100,
            Taxes = 20,
            IsRefundable = false,
            SeatNumber = "12A",
            PassengerFullName = "John Doe",
            PassengerEmail = "john@example.com"
        };
    }

    protected Booking CreateBooking(int id = 10, string userId = "user-1", string status = "Active")
    {
        return new Booking
        {
            Id = id,
            UserId = userId,
            BookingStatus = new BookingStatus { Status = status }
        };
    }

    protected FlightSchedule CreateFlightSchedule(int id = 20)
    {
        return new FlightSchedule { Id = id };
    }

    protected void CaptureAddTicket()
    {
        _capturedAddedTicket = null;

        _ticketRepository
            .Setup(x => x.AddAsync(It.IsAny<Ticket>()))
            .Callback<Ticket>(t => _capturedAddedTicket = t)
            .Returns(Task.CompletedTask);
    }

    protected void CaptureIncrementQuantity()
    {
        _capturedIncrementBookingId = null;

        _bookingRepository
            .Setup(x => x.IncrementQuantityAsync(It.IsAny<int>()))
            .Callback<int>(id => _capturedIncrementBookingId = id)
            .Returns(Task.CompletedTask);
    }

    protected void CaptureDeleteTicketId()
    {
        _capturedDeletedTicketId = null;

        _ticketRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .Callback<int>(id => _capturedDeletedTicketId = id)
            .ReturnsAsync(true);
    }
}