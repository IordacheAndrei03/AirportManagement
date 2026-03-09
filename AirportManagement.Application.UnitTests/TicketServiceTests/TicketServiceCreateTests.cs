using AirportManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.TicketServiceTests
{
    public class TicketServiceCreateTests : TicketServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_WhenBasePriceNotPositive_ReturnsInvalid()
        {
            var dto = CreateValidTicketCreateRequestDto();
            dto.BasePrice = 0;

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("BasePrice", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenTaxesNegative_ReturnsInvalid()
        {
            var dto = CreateValidTicketCreateRequestDto();
            dto.Taxes = -1;

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Taxes", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .Returns(Task.FromResult<Booking?>(null));

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Booking", result.Error!, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenBookingBelongsToAnotherUser_ReturnsForbidden()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "other-user", status: "Active"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("another user's", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenBookingNotActive_ReturnsConflict()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "user-1", status: "Canceled"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("non-active", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenFlightScheduleNotFound_ReturnsNotFound()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "user-1", status: "Active"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            _flightScheduleRepository
                .Setup(x => x.GetByIdAsync(dto.FlightScheduleId))
                .Returns(Task.FromResult<FlightSchedule?>(null));

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("FlightSchedule", result.Error!, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenCapacityInvalid_ReturnsInvalid()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "user-1", status: "Active"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            _flightScheduleRepository
                .Setup(x => x.GetByIdAsync(dto.FlightScheduleId))
                .ReturnsAsync(CreateFlightSchedule(dto.FlightScheduleId));

            _flightScheduleRepository
                .Setup(x => x.GetSeatCapacityAsync(dto.FlightScheduleId))
                .ReturnsAsync(0);

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("capacity", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenNotEnoughSeats_ReturnsConflict()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "user-1", status: "Active"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            _flightScheduleRepository
                .Setup(x => x.GetByIdAsync(dto.FlightScheduleId))
                .ReturnsAsync(CreateFlightSchedule(dto.FlightScheduleId));

            _flightScheduleRepository
                .Setup(x => x.GetSeatCapacityAsync(dto.FlightScheduleId))
                .ReturnsAsync(10);

            _flightScheduleRepository
                .Setup(x => x.GetActiveBookedSeatsAsync(dto.FlightScheduleId))
                .ReturnsAsync(10);

            var result = await _ticketService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Not enough seats", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenValid_AddsTicket_IncrementsQuantity_AndReturnsSuccess()
        {
            var dto = CreateValidTicketCreateRequestDto();

            _bookingRepository
                .Setup(x => x.GetByIdWithStatusAsync(dto.BookingId))
                .ReturnsAsync(CreateBooking(id: dto.BookingId, userId: "user-1", status: "Active"));

            _currentUserService.SetupGet(x => x.UserId).Returns("user-1");

            _flightScheduleRepository
                .Setup(x => x.GetByIdAsync(dto.FlightScheduleId))
                .ReturnsAsync(CreateFlightSchedule(dto.FlightScheduleId));

            _flightScheduleRepository
                .Setup(x => x.GetSeatCapacityAsync(dto.FlightScheduleId))
                .ReturnsAsync(100);

            _flightScheduleRepository
                .Setup(x => x.GetActiveBookedSeatsAsync(dto.FlightScheduleId))
                .ReturnsAsync(5);

            CaptureAddTicket();
            CaptureIncrementQuantity();

            var result = await _ticketService.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.NotNull(_capturedAddedTicket);
            Assert.Equal(dto.BookingId, _capturedAddedTicket!.BookingId);
            Assert.Equal(dto.FlightScheduleId, _capturedAddedTicket.FlightScheduleId);
            Assert.Equal(dto.FareClass, _capturedAddedTicket.FareClass);
            Assert.Equal(dto.BasePrice, _capturedAddedTicket.BasePrice);
            Assert.Equal(dto.Taxes, _capturedAddedTicket.Taxes);
            Assert.Equal(dto.BasePrice + dto.Taxes, _capturedAddedTicket.TotalPrice);
            Assert.Equal("EUR", _capturedAddedTicket.Currency);
            Assert.Equal(dto.IsRefundable, _capturedAddedTicket.IsRefundable);
            Assert.Equal(dto.SeatNumber, _capturedAddedTicket.SeatNumber);
            Assert.Equal(dto.PassengerFullName, _capturedAddedTicket.PassangerFullName);
            Assert.Equal(dto.PassengerEmail, _capturedAddedTicket.PassangerEmail);

            Assert.Equal(dto.FareClass, result.Value!.FareClass);
            Assert.Equal(dto.BasePrice + dto.Taxes, result.Value.TotalPrice);
            Assert.Equal(dto.IsRefundable, result.Value.IsRefundable);
            Assert.Equal(dto.SeatNumber, result.Value.SeatNumber);
            Assert.Equal(dto.PassengerFullName, result.Value.PassengerFullName);

            Assert.Equal(dto.BookingId, _capturedIncrementBookingId);
        }
    }
}
