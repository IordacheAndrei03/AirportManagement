using AirportManagement.Application;
using AirportManagement.Application.Dtos.Flights;
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
    public class FlightsControllerCreateTests : FlightsControllerTestBase
    {
        [Fact]
        public async Task Create_WhenOk_Returns201_WithResultObjectInBody()
        {
            var dto = CreateValidFlightCreateDto();

            var serviceResult = ResultObject<int>.Success(123);

            _flightService.Setup(s => s.CreateFlightAsync(dto))
                .ReturnsAsync(serviceResult);

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(201, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<int>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.Equal(123, returned.Value);

            _flightService.Verify(s => s.CreateFlightAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenInvalid_Returns400()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.CreateFlightAsync(dto))
                .ReturnsAsync(ResultObject<int>.Invalid("invalid"));

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<int>>(badRequest.Value);
        }

        [Fact]
        public async Task Create_WhenConflict_Returns409()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.CreateFlightAsync(dto))
                .ReturnsAsync(ResultObject<int>.Conflict("conflict"));

            var result = await _controller.Create(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<int>>(conflict.Value);
        }

        [Fact]
        public async Task Create_WhenForbidden_Returns403()
        {
            var dto = CreateValidFlightCreateDto();

            _flightService.Setup(s => s.CreateFlightAsync(dto))
                .ReturnsAsync(ResultObject<int>.Forbidden("forbidden"));

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<int>>(objectResult.Value);
        }

    }
}
