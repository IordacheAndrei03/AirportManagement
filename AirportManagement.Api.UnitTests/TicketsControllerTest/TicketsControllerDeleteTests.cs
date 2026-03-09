using AirportManagement.Application.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.TicketsControllerTest
{
    public class TicketsControllerDeleteTests : TicketsControllerTestBase
    {
        [Fact]
        public async Task Delete_WhenOk_Returns200_WithResultInBody()
        {
            _ticketService.Setup(s => s.DeleteAsync(10))
                .ReturnsAsync(Result.Success());

            var result = await _controller.Delete(10);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<Result>(ok.Value);

            _ticketService.Verify(s => s.DeleteAsync(10), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenNotFound_Returns404()
        {
            _ticketService.Setup(s => s.DeleteAsync(10))
                .ReturnsAsync(Result.NotFound("not found"));

            var result = await _controller.Delete(10);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<Result>(notFound.Value);
        }

        [Fact]
        public async Task Delete_WhenInvalid_Returns400()
        {
            _ticketService.Setup(s => s.DeleteAsync(10))
                .ReturnsAsync(Result.Invalid("invalid"));

            var result = await _controller.Delete(10);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<Result>(badRequest.Value);
        }

        [Fact]
        public async Task Delete_WhenConflict_Returns409()
        {
            _ticketService.Setup(s => s.DeleteAsync(10))
                .ReturnsAsync(Result.Conflict("conflict"));

            var result = await _controller.Delete(10);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<Result>(conflict.Value);
        }

        [Fact]
        public async Task Delete_WhenForbidden_Returns403()
        {
            _ticketService.Setup(s => s.DeleteAsync(10))
                .ReturnsAsync(Result.Forbidden("forbidden"));

            var result = await _controller.Delete(10);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<Result>(objectResult.Value);
        }
    }
}
