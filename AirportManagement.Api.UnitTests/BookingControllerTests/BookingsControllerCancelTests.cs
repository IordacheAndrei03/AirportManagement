using AirportManagement.Application.Enums;
using AirportManagement.Application.Results;
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
    public class BookingsControllerCancelTests : BookingsControllerTestBase
    {
        [Fact]
        public async Task Cancel_WhenOk_Returns200_WithResultInBody()
        {
            _bookingService.Setup(s => s.CancelAsync("ABC"))
                .ReturnsAsync(Result.Success());

            var result = await _controller.Cancel("ABC");

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Result>(ok.Value);
            Assert.Equal(ResultStatus.Ok, returned.Status);

            _bookingService.Verify(s => s.CancelAsync("ABC"), Times.Once);
        }

        [Fact]
        public async Task Cancel_WhenInvalid_Returns400()
        {
            _bookingService.Setup(s => s.CancelAsync("ABC"))
                .ReturnsAsync(Result.Invalid("invalid"));

            var result = await _controller.Cancel("ABC");

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<Result>(badRequest.Value);
        }

        [Fact]
        public async Task Cancel_WhenNotFound_Returns404()
        {
            _bookingService.Setup(s => s.CancelAsync("ABC"))
                .ReturnsAsync(Result.NotFound("not found"));

            var result = await _controller.Cancel("ABC");

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<Result>(notFound.Value);
        }

        [Fact]
        public async Task Cancel_WhenConflict_Returns409()
        {
            _bookingService.Setup(s => s.CancelAsync("ABC"))
                .ReturnsAsync(Result.Conflict("conflict"));

            var result = await _controller.Cancel("ABC");

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<Result>(conflict.Value);
        }

        [Fact]
        public async Task Cancel_WhenForbidden_Returns403()
        {
            _bookingService.Setup(s => s.CancelAsync("ABC"))
                .ReturnsAsync(Result.Forbidden("forbidden"));

            var result = await _controller.Cancel("ABC");

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<Result>(objectResult.Value);
        }
    }
}