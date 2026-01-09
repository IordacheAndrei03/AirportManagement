using AirportManagement.Application.Enums;
using Moq;
using System;
using Xunit;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using DomainTicket = AirportManagement.Domain.Entities.Ticket;

namespace AirportManagement.Application.UnitTests.BookingServiceTests
{
    public class BookingServiceGetByCodeTets : BookingServiceTestBase
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetByCodeAsync_WhenCodeInvalid_ReturnsInvalid_AndDoesNotCallRepos(string? code)
        {
            var result = await _sut.GetByCodeAsync(code!);

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Equal("Booking code is required.", result.Error);
            _bookingRepository.Verify(r => r.GetByConfirmationCodeAsync(It.IsAny<string>()), Times.Never);
            _ticketRepository.Verify(r => r.GetByBookingIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(null));

            var result = await _sut.GetByCodeAsync("ABC");

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.NotFound, result.Status);
            Assert.Contains("ABC", result.Error!);
            _ticketRepository.Verify(r => r.GetByBookingIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenTicketIsNull_ReturnsDtoWithTotalAmountZero()
        {
            var booking = new DomainBooking
            {
                Id = 1,
                UserId = "user-1",
                BookingStatusId = 10,
                CreatedUtc = DateTime.UtcNow,
                ConfirmationCode = "ABC",
                Quantity = 2,
                BookingStatus = new DomainBookingStatus { Id = 10, Status = "Active" }
            };
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));
            _ticketRepository.Setup(r => r.GetByBookingIdAsync(1))
                .Returns(Task.FromResult<DomainTicket?>(null));

            var result = await _sut.GetByCodeAsync("ABC");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("ABC", result.Value!.ConfirmationCode);
            Assert.Equal("Active", result.Value.Status);
            Assert.Equal(0m, result.Value.TotalAmount);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenTicketExists_ReturnsFullDto_AndCalculatesTotal()
        {
            var booking = new DomainBooking
            {
                Id = 1,
                UserId = "user-1",
                BookingStatusId = 10,
                CreatedUtc = DateTime.UtcNow,
                ConfirmationCode = "ABC",
                Quantity = 2,
                BookingStatus = new DomainBookingStatus { Id = 10, Status = "Active" }
            };

            var ticket = new DomainTicket
            {
                Id = 10,
                BookingId = 1,
                FlightScheduleId = 5,
                PassangerFullName = "John Doe",
                PassangerEmail = "john@doe.com",
                TotalPrice = 100m,
                Currency = "EUR",
                FareClass = "Y",
                SeatNumber = "1A",
                Booking = null!,
                FlightSchedule = null!
            };

            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            _ticketRepository.Setup(r => r.GetByBookingIdAsync(1))
                .Returns(Task.FromResult<DomainTicket?>(ticket));

            var result = await _sut.GetByCodeAsync("ABC");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(200m, result.Value!.TotalAmount);
            Assert.Equal(10, result.Value.TicketId);
            Assert.Equal(5, result.Value.FlightScheduleId);
            Assert.Equal("John Doe", result.Value.PassengerFullName);
            Assert.Equal("john@doe.com", result.Value.PassengerEmail);
            Assert.Equal("EUR", result.Value.Currency);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenBookingStatusIsNull_ReturnsUnknownStatus()
        {
            var booking = new DomainBooking
            {
                Id = 1,
                UserId = "user-1",
                BookingStatusId = 10,
                CreatedUtc = DateTime.UtcNow,
                ConfirmationCode = "ABC",
                Quantity = 1,
                BookingStatus = null! 
            };

            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            _ticketRepository.Setup(r => r.GetByBookingIdAsync(1))
                .Returns(Task.FromResult<DomainTicket?>(null));

            var result = await _sut.GetByCodeAsync("ABC");

            Assert.True(result.IsSuccess);
            Assert.Equal("Unknown", result.Value!.Status);
        }
    }
}
