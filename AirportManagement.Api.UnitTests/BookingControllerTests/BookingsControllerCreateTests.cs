using AirportManagement.Application;
using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.BookingControllerTests
{
    public class BookingsControllerCreateTests : BookingsControllerTestBase
    {
        [Fact]
        public async Task Create_WhenOk_Returns201_WithResultObjectInBody()
        {
            var dto = CreateValidBookingCreateRequestDto();

            var response = new BookingCreateResponseDto
            {
                ConfirmationCode = "ABC123",
                Quantity = 1,
                Status = "Active"
            };

            var serviceResult = ResultObject<BookingCreateResponseDto>.Success(response);

            _bookingService.Setup(s => s.CreateAsync())
                .ReturnsAsync(serviceResult);

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<BookingCreateResponseDto>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.Equal("ABC123", returned.Value!.ConfirmationCode);

            _bookingService.Verify(s => s.CreateAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_WhenInvalid_Returns400_WithResultObjectInBody()
        {
            var dto = CreateValidBookingCreateRequestDto();

            var serviceResult = ResultObject<BookingCreateResponseDto>.Invalid("some error");

            _bookingService.Setup(s => s.CreateAsync())
                .ReturnsAsync(serviceResult);

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<BookingCreateResponseDto>>(badRequest.Value);

            Assert.False(returned.IsSuccess);
            Assert.Equal(ResultStatus.Invalid, returned.Status);

            _bookingService.Verify(s => s.CreateAsync(), Times.Once);
        }

        [Fact]
        public async Task Create_WhenConflict_Returns409()
        {
            var dto = CreateValidBookingCreateRequestDto();

            _bookingService.Setup(s => s.CreateAsync())
                .ReturnsAsync(ResultObject<BookingCreateResponseDto>.Conflict("conflict"));

            var result = await _controller.Create(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<BookingCreateResponseDto>>(conflict.Value);
        }

        [Fact]
        public async Task Create_WhenForbidden_Returns403()
        {
            var dto = CreateValidBookingCreateRequestDto();

            _bookingService.Setup(s => s.CreateAsync())
                .ReturnsAsync(ResultObject<BookingCreateResponseDto>.Forbidden("forbidden"));

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<BookingCreateResponseDto>>(objectResult.Value);
        }
    }
}
