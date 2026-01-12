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
    public class FlightsControllerGetByIdTests : FlightsControllerTestBase
    {
        [Fact]
        public async Task GetById_WhenOk_Returns200_WithResultObjectInBody()
        {
            var dto = new FlightDetailsDto();
            var serviceResult = ResultObject<FlightDetailsDto>.Success(dto);

            _flightService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(serviceResult);

            var result = await _controller.GetById(10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<FlightDetailsDto>>(ok.Value);

            Assert.True(returned.IsSuccess);

            _flightService.Verify(s => s.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenNotFound_Returns404()
        {
            _flightService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightDetailsDto>.NotFound("not found"));

            var result = await _controller.GetById(10);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightDetailsDto>>(notFound.Value);
        }

        [Fact]
        public async Task GetById_WhenInvalid_Returns400()
        {
            _flightService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightDetailsDto>.Invalid("invalid"));

            var result = await _controller.GetById(10);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightDetailsDto>>(badRequest.Value);
        }

        [Fact]
        public async Task GetById_WhenConflict_Returns409()
        {
            _flightService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightDetailsDto>.Conflict("conflict"));

            var result = await _controller.GetById(10);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightDetailsDto>>(conflict.Value);
        }

        [Fact]
        public async Task GetById_WhenForbidden_Returns403()
        {
            _flightService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightDetailsDto>.Forbidden("forbidden"));

            var result = await _controller.GetById(10);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<FlightDetailsDto>>(objectResult.Value);
        }
    }
}
