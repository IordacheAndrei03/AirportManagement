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
            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(ReturnNull<DomainBooking>());

            var result = await _sut.CancelAsync(DefaultCode);

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.NotFound, result.Status);
            Assert.Contains(DefaultCode, result.Error!);
        }

        [Fact]
        public async Task CancelAsync_WhenCancelledStatusMissing_ReturnsInvalid_AndDoesNotSave()
        {
            var booking = CreateBooking(id: 1, code: DefaultCode, statusId: 10, userId: DefaultUserId);
            ArrangeStatus(CancelledStatusName, status: null);

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            var result = await _sut.CancelAsync(DefaultCode);

            Assert.False(result.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, result.Status);
            Assert.Contains(CancelledStatusName, result.Error!);

            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenAlreadyCancelled_ReturnsSuccess_AndDoesNotSave()
        {
            var cancelled = CreateStatus(id: 99, status: CancelledStatusName);
            var booking = CreateBooking(id: 1, code: DefaultCode, statusId: 99, userId: DefaultUserId);
            ArrangeStatus(CancelledStatusName, cancelled);

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            var result = await _sut.CancelAsync(DefaultCode);

            Assert.True(result.IsSuccess);
            Assert.Equal(ResultStatus.Ok, result.Status);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelAsync_WhenNotCancelled_UpdatesStatusId_AndSaves()
        {
            var cancelled = CreateStatus(id: 99, status: CancelledStatusName);
            var booking = CreateBooking(id: 1, code: DefaultCode, statusId: 10, userId: DefaultUserId);
            ArrangeStatus(CancelledStatusName, cancelled);

            _bookingRepository
                .Setup(r => r.GetByConfirmationCodeAsync(DefaultCode))
                .Returns(Task.FromResult<DomainBooking?>(booking));

            var result = await _sut.CancelAsync(DefaultCode);

            Assert.True(result.IsSuccess);
            Assert.Equal(99, booking.BookingStatusId);
            _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
