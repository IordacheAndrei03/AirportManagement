using System;
using System.Collections.Generic;
using System.Linq;
using AirportManagement.Application.Enums;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using Moq;
using Xunit;

namespace AirportManagement.Application.UnitTests.BookingServiceTests
{
    public class BookingServiceCancelTests : BookingServiceTestBase
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task CancelAsync_WhenCodeInvalid_ReturnsInvalid(string? code)
        {
            var result = await _sut.CancelAsync(code!);

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Equal("Booking code is required.", result.Error);
            _bookingRepository.Verify(r => r.GetByConfirmationCodeAsync(It.IsAny<string>()), Times.Never);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenBookingNotFound_ReturnsNotFound()
        {
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(null));

            var result = await _sut.CancelAsync("ABC");

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.NotFound, result.Status);
            Assert.Contains("ABC", result.Error!);
        }

        [Fact]
        public async Task CancelAsync_WhenCancelledStatusMissing_ReturnsInvalid_AndDoesNotSave()
        {
            var booking = new DomainBooking { Id = 1, ConfirmationCode = "ABC", BookingStatusId = 10, UserId = "user-1" };
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));
            _bookingRepository.Setup(r => r.GetByStatusAsync("Cancelled"))
                .Returns(Task.FromResult<DomainBookingStatus?>(null));

            var result = await _sut.CancelAsync("ABC");

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Contains("Cancelled", result.Error!);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenAlreadyCancelled_ReturnsSuccess_AndDoesNotSave()
        {
            var cancelled = new DomainBookingStatus { Id = 99, Status = "Cancelled" };
            var booking = new DomainBooking { Id = 1, ConfirmationCode = "ABC", BookingStatusId = 99, UserId = "user-1" };
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));
            _bookingRepository.Setup(r => r.GetByStatusAsync("Cancelled"))
                .Returns(Task.FromResult<DomainBookingStatus?>(cancelled));

            var result = await _sut.CancelAsync("ABC");

            Assert.True(result.IsSuccess);
            Assert.Equal(ResultStatus.Ok, result.Status);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenNotCancelled_UpdatesStatusId_AndSaves()
        {
            var cancelled = new DomainBookingStatus { Id = 99, Status = "Cancelled" };
            var booking = new DomainBooking { Id = 1, ConfirmationCode = "ABC", BookingStatusId = 10, UserId = "user-1" };
            _bookingRepository.Setup(r => r.GetByConfirmationCodeAsync("ABC"))
                .Returns(Task.FromResult<DomainBooking?>(booking));
            _bookingRepository.Setup(r => r.GetByStatusAsync("Cancelled"))
                .Returns(Task.FromResult<DomainBookingStatus?>(cancelled));
            _unitOfWork.Setup(u => u.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            var result = await _sut.CancelAsync("ABC");

            Assert.True(result.IsSuccess);
            Assert.Equal(99, booking.BookingStatusId);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
