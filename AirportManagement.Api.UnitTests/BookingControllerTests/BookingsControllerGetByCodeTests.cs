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
    public class BookingsControllerGetByCodeTests : BookingsControllerTestBase
    {
        [Fact]
        public async Task GetByCode_WhenOk_Returns200_WithResultObjectInBody()
        {
            var dto = new BookingDetailsDto { ConfirmationCode = "ABC" };
            var serviceResult = ResultObject<BookingDetailsDto>.Success(dto);

            _bookingService.Setup(s => s.GetByCodeAsync("ABC"))
                .ReturnsAsync(serviceResult);

            var result = await _controller.GetByCode("ABC");

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<BookingDetailsDto>>(ok.Value);

            Assert.True(returned.IsSuccess);
            Assert.Equal("ABC", returned.Value!.ConfirmationCode);

            _bookingService.Verify(s => s.GetByCodeAsync("ABC"), Times.Once);
        }

        [Fact]
        public async Task GetByCode_WhenInvalid_Returns400()
        {
            _bookingService.Setup(s => s.GetByCodeAsync("   "))
                .ReturnsAsync(ResultObject<BookingDetailsDto>.Invalid("invalid"));

            var result = await _controller.GetByCode("   ");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<BookingDetailsDto>>(badRequest.Value);
            Assert.Equal(ResultStatus.Invalid, returned.Status);
        }

        [Fact]
        public async Task GetByCode_WhenNotFound_Returns404()
        {
            _bookingService.Setup(s => s.GetByCodeAsync("ABC"))
                .ReturnsAsync(ResultObject<BookingDetailsDto>.NotFound("not found"));

            var result = await _controller.GetByCode("ABC");

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<BookingDetailsDto>>(notFound.Value);
        }

        [Fact]
        public async Task GetByCode_WhenConflict_Returns409()
        {
            _bookingService.Setup(s => s.GetByCodeAsync("ABC"))
                .ReturnsAsync(ResultObject<BookingDetailsDto>.Conflict("conflict"));

            var result = await _controller.GetByCode("ABC");

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<BookingDetailsDto>>(conflict.Value);
        }

        [Fact]
        public async Task GetByCode_WhenForbidden_Returns403()
        {
            _bookingService.Setup(s => s.GetByCodeAsync("ABC"))
                .ReturnsAsync(ResultObject<BookingDetailsDto>.Forbidden("forbidden"));

            var result = await _controller.GetByCode("ABC");

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<BookingDetailsDto>>(objectResult.Value);
        }
    }
}