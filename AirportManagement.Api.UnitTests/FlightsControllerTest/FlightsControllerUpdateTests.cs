using AirportManagement.Application.Dtos.Flights;
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

namespace AirportManagement.Api.UnitTests.FlightsControllerTest
{
    public class FlightsControllerUpdateTests : FlightsControllerTestBase
    {
      
        [Fact]
        public async Task Update_WhenOk_Returns200_WithResultInBody()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.UpdateAsync(10, dto))
                .ReturnsAsync(Result.Success());

            var result = await _controller.Update(10, dto);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Result>(ok.Value);
            Assert.Equal(ResultStatus.Ok, returned.Status);

            _flightService.Verify(s => s.UpdateAsync(10, dto), Times.Once);
        }

        [Fact]
        public async Task Update_WhenNotFound_Returns404()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.UpdateAsync(10, dto))
                .ReturnsAsync(Result.NotFound("not found"));

            var result = await _controller.Update(10, dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<Result>(notFound.Value);
        }

        [Fact]
        public async Task Update_WhenInvalid_Returns400()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.UpdateAsync(10, dto))
                .ReturnsAsync(Result.Invalid("invalid"));

            var result = await _controller.Update(10, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<Result>(badRequest.Value);
        }

        [Fact]
        public async Task Update_WhenConflict_Returns409()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.UpdateAsync(10, dto))
                .ReturnsAsync(Result.Conflict("conflict"));

            var result = await _controller.Update(10, dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<Result>(conflict.Value);
        }

        [Fact]
        public async Task Update_WhenForbidden_Returns403()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.UpdateAsync(10, dto))
                .ReturnsAsync(Result.Forbidden("forbidden"));

            var result = await _controller.Update(10, dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<Result>(objectResult.Value);
        }
    }
}
