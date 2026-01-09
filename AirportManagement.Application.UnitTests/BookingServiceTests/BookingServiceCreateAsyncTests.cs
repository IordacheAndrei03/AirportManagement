using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using Moq;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using DomainTicket = AirportManagement.Domain.Entities.Ticket;
using Xunit;

namespace AirportManagement.Application.UnitTests.BookingServiceTests
{
    public class BookingServiceCreateAsyncTests : BookingServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_WhenActiveStatusMissing_ReturnsInvalid_AndDoesNotSave()
        {
            _bookingRepository.Setup(r => r.GetByStatusAsync("Active"))
                .Returns(Task.FromResult<DomainBookingStatus?>(null));

            var result = await _sut.CreateAsync();

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Contains("Active", result.Error!);
            _bookingRepository.Verify(r => r.AddAsync(It.IsAny<DomainBooking>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenActiveStatusExists_ReturnsSuccessResponse()
        {
            _bookingRepository.Setup(r => r.GetByStatusAsync("Active"))
                .Returns(Task.FromResult<DomainBookingStatus?>(new DomainBookingStatus { Id = 10, Status = "Active" }));
            _bookingRepository.Setup(r => r.AddAsync(It.IsAny<DomainBooking>()))
                .Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            var result = await _sut.CreateAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Active", result.Value!.Status);
            Assert.Equal(1, result.Value.Quantity);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.ConfirmationCode));
            Assert.Equal(6, result.Value.ConfirmationCode.Length);
        }

        [Fact]
        public async Task CreateAsync_WhenActiveStatusExists_AddsBookingWithExpectedFields()
        {
            _bookingRepository.Setup(r => r.GetByStatusAsync("Active"))
                .Returns(Task.FromResult<DomainBookingStatus?>(new DomainBookingStatus { Id = 10, Status = "Active" }));
            DomainBooking? captured = null;
            _bookingRepository.Setup(r => r.AddAsync(It.IsAny<DomainBooking>()))
                .Callback<DomainBooking>(b => captured = b)
                .Returns(Task.CompletedTask);
            _unitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(1));
            var before = DateTime.UtcNow;

            var result = await _sut.CreateAsync();
            var after = DateTime.UtcNow;

            Assert.True(result.IsSuccess);
            Assert.NotNull(captured);
            Assert.Equal("user-1", captured!.UserId);
            Assert.Equal(10, captured.BookingStatusId);
            Assert.Equal(1, captured.Quantity);
            Assert.InRange(captured.CreatedUtc, before.AddSeconds(-2), after.AddSeconds(2));
            Assert.Equal(result.Value!.ConfirmationCode, captured.ConfirmationCode);
            _bookingRepository.Verify(r => r.AddAsync(It.IsAny<DomainBooking>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}

