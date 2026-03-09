using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Services;
using Moq;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using DomainTicket = AirportManagement.Domain.Entities.Ticket;

namespace AirportManagement.Application.UnitTests.BookingServiceTests
{
    public abstract class BookingServiceTestBase
    {
        protected const string DefaultUserId = "user-1";
        protected const string DefaultCode = "ABC";
        protected const string ActiveStatusName = "Active";
        protected const string CancelledStatusName = "Cancelled";

        protected readonly Mock<IUnitOfWork> _unitOfWork;
        protected readonly Mock<IBookingRepository> _bookingRepository;
        protected readonly Mock<ITicketRepository> _ticketRepository;
        protected readonly Mock<ICurrentUserService> _currentUserService;

        protected readonly BookingService _sut;

        protected DomainBooking? _capturedAddedBooking;

        protected BookingServiceTestBase()
        {
            _unitOfWork = new Mock<IUnitOfWork>(MockBehavior.Loose);
            _bookingRepository = new Mock<IBookingRepository>(MockBehavior.Loose);
            _ticketRepository = new Mock<ITicketRepository>(MockBehavior.Loose);
            _currentUserService = new Mock<ICurrentUserService>(MockBehavior.Loose);

            _unitOfWork.SetupGet(u => u.BookingRepository).Returns(_bookingRepository.Object);
            _unitOfWork.SetupGet(u => u.TicketRepository).Returns(_ticketRepository.Object);
            _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            _currentUserService.SetupGet(c => c.UserId).Returns(DefaultUserId);

            _sut = new BookingService(_unitOfWork.Object, _currentUserService.Object);
        }

        protected Task<T?> ReturnNull<T>() where T : class
            => Task.FromResult<T?>(null);

        protected DomainBookingStatus CreateStatus(int id, string status)
            => new DomainBookingStatus { Id = id, Status = status };

        protected DomainBooking CreateBooking(
            int id = 1,
            string code = DefaultCode,
            int statusId = 10,
            string userId = DefaultUserId,
            int quantity = 1,
            DomainBookingStatus? status = null,
            DateTime? createdUtc = null)
        {
            return new DomainBooking
            {
                Id = id,
                ConfirmationCode = code,
                BookingStatusId = statusId,
                UserId = userId,
                Quantity = quantity,
                CreatedUtc = createdUtc ?? DateTime.UtcNow,
                BookingStatus = status!
            };
        }

        protected DomainTicket CreateTicket(
            int id = 10,
            int bookingId = 1,
            int scheduleId = 5,
            decimal totalPrice = 100m,
            string currency = "EUR",
            string passengerName = "John Doe",
            string passengerEmail = "john@doe.com")
        {
            return new DomainTicket
            {
                Id = id,
                BookingId = bookingId,
                FlightScheduleId = scheduleId,
                PassangerFullName = passengerName,
                PassangerEmail = passengerEmail,
                TotalPrice = totalPrice,
                Currency = currency,
                FareClass = "Y",
                SeatNumber = "1A",
                Booking = null!,
                FlightSchedule = null!
            };
        }

        protected void CaptureAddedBooking()
        {
            _capturedAddedBooking = null;

            _bookingRepository
                .Setup(r => r.AddAsync(It.IsAny<DomainBooking>()))
                .Callback<DomainBooking>(b => _capturedAddedBooking = b)
                .Returns(Task.CompletedTask);
        }

        protected void ArrangeStatus(string statusName, DomainBookingStatus? status)
        {
            _bookingRepository
                .Setup(r => r.GetByStatusAsync(statusName))
                .Returns(Task.FromResult(status));
        }
    }
}
