using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using Moq;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using Xunit;

namespace AirportManagement.Application.UnitTests.BookingServiceTests
{
    public class BookingServiceCreateAsyncTests : BookingServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_WhenActiveStatusMissing_ReturnsInvalid_AndDoesNotSave()
        {
            ArrangeStatus(ActiveStatusName, status: null);

            var result = await _sut.CreateAsync();

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Contains(ActiveStatusName, result.Error!);

            _bookingRepository.Verify(r => r.AddAsync(It.IsAny<DomainBooking>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenActiveStatusExists_ReturnsSuccessResponse()
        {
            var active = CreateStatus(id: 10, status: ActiveStatusName);
            ArrangeStatus(ActiveStatusName, active);

            _bookingRepository
                .Setup(r => r.AddAsync(It.IsAny<DomainBooking>()))
                .Returns(Task.CompletedTask);

            var result = await _sut.CreateAsync();

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(ActiveStatusName, result.Value!.Status);
            Assert.Equal(1, result.Value.Quantity);
            Assert.False(string.IsNullOrWhiteSpace(result.Value.ConfirmationCode));
            Assert.Equal(6, result.Value.ConfirmationCode.Length);
        }

        [Fact]
        public async Task CreateAsync_WhenActiveStatusExists_AddsBookingWithExpectedFields()
        {
            var active = CreateStatus(id: 10, status: ActiveStatusName);
            ArrangeStatus(ActiveStatusName, active);
            CaptureAddedBooking();
            var before = DateTime.UtcNow;

            var result = await _sut.CreateAsync();
            var after = DateTime.UtcNow;

            Assert.True(result.IsSuccess);
            Assert.NotNull(_capturedAddedBooking);
            Assert.Equal(DefaultUserId, _capturedAddedBooking!.UserId);
            Assert.Equal(10, _capturedAddedBooking.BookingStatusId);
            Assert.Equal(1, _capturedAddedBooking.Quantity);
            Assert.InRange(_capturedAddedBooking.CreatedUtc, before.AddSeconds(-2), after.AddSeconds(2));
            Assert.Equal(result.Value!.ConfirmationCode, _capturedAddedBooking.ConfirmationCode);

            _bookingRepository.Verify(r => r.AddAsync(It.IsAny<DomainBooking>()), Times.Once);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}

