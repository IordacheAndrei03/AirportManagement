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
            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(ReturnNull<DomainBooking>());

            var result = await _sut.GetByCodeAsync(DefaultCode);

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.NotFound, result.Status);
            Assert.Contains(DefaultCode, result.Error!);
            _ticketRepository.Verify(r => r.GetByBookingIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenTicketIsNull_ReturnsDtoWithTotalAmountZero()
        {
            var booking = CreateBooking(
                id: 1,
                code: DefaultCode,
                statusId: 10,
                userId: DefaultUserId,
                quantity: 2,
                status: CreateStatus(10, ActiveStatusName));

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            _ticketRepository
                .Setup(r => r.GetByBookingIdAsync(1))
                .Returns(ReturnNull<DomainTicket>());

            var result = await _sut.GetByCodeAsync(DefaultCode);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(DefaultCode, result.Value!.ConfirmationCode);
            Assert.Equal(ActiveStatusName, result.Value.Status);
            Assert.Equal(0m, result.Value.TotalAmount);
        }

        [Fact]
        public async Task GetByCodeAsync_WhenTicketExists_ReturnsFullDto_AndCalculatesTotal()
        {
            var booking = CreateBooking(
                id: 1,
                code: DefaultCode,
                statusId: 10,
                userId: DefaultUserId,
                quantity: 2,
                status: CreateStatus(10, ActiveStatusName));

            var ticket = CreateTicket(
                id: 10,
                bookingId: 1,
                scheduleId: 5,
                totalPrice: 100m,
                currency: "EUR",
                passengerName: "John Doe",
                passengerEmail: "john@doe.com");

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            _ticketRepository
                .Setup(r => r.GetByBookingIdAsync(1))
                .Returns(Task.FromResult<DomainTicket?>(ticket));

            var result = await _sut.GetByCodeAsync(DefaultCode);

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
            var booking = CreateBooking(
                id: 1,
                code: DefaultCode,
                statusId: 10,
                userId: DefaultUserId,
                quantity: 1,
                status: null);

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            _ticketRepository
                .Setup(r => r.GetByBookingIdAsync(1))
                .Returns(ReturnNull<DomainTicket>());

            var result = await _sut.GetByCodeAsync(DefaultCode);

            Assert.True(result.IsSuccess);
            Assert.Equal("Unknown", result.Value!.Status);
        }
    }
}
