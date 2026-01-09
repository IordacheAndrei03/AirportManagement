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
        protected readonly Mock<IUnitOfWork> _unitOfWork;
        protected readonly Mock<IBookingRepository> _bookingRepository;
        protected readonly Mock<ITicketRepository> _ticketRepository;
        protected readonly Mock<ICurrentUserService> _currentUserService;

        protected readonly BookingService _sut;

        protected BookingServiceTestBase()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _bookingRepository = new Mock<IBookingRepository>();
            _ticketRepository = new Mock<ITicketRepository>();
            _currentUserService = new Mock<ICurrentUserService>();

            _unitOfWork.SetupGet(u => u.BookingRepository).Returns(_bookingRepository.Object);
            _unitOfWork.SetupGet(u => u.TicketRepository).Returns(_ticketRepository.Object);

            _currentUserService.SetupGet(c => c.UserId).Returns("user-1");

            _sut = new BookingService(_unitOfWork.Object, _currentUserService.Object);
        }
    }
}
