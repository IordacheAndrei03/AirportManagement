using AirportManagement.Application;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.FlightScheduleControllerTests
{
    public class FlightScheduleControllerGetByIdTests : FlightScheduleControllerTestBase
    {
        [Fact]
        public async Task GetById_WhenOk_Returns200_WithResultObjectInBody()
        {
            var dto = new FlightScheduleDetailsDto();
            _scheduleService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightScheduleDetailsDto>.Success(dto));

            var result = await _controller.GetById(10);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<FlightScheduleDetailsDto>>(ok.Value);
            Assert.True(returned.IsSuccess);

            _scheduleService.Verify(s => s.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenNotFound_Returns404()
        {
            _scheduleService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightScheduleDetailsDto>.NotFound("not found"));

            var result = await _controller.GetById(10);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightScheduleDetailsDto>>(notFound.Value);
        }

        [Fact]
        public async Task GetById_WhenInvalid_Returns400()
        {
            _scheduleService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightScheduleDetailsDto>.Invalid("invalid"));

            var result = await _controller.GetById(10);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightScheduleDetailsDto>>(badRequest.Value);
        }

        [Fact]
        public async Task GetById_WhenConflict_Returns409()
        {
            _scheduleService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightScheduleDetailsDto>.Conflict("conflict"));

            var result = await _controller.GetById(10);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<FlightScheduleDetailsDto>>(conflict.Value);
        }

        [Fact]
        public async Task GetById_WhenForbidden_Returns403()
        {
            _scheduleService.Setup(s => s.GetByIdAsync(10))
                .ReturnsAsync(ResultObject<FlightScheduleDetailsDto>.Forbidden("forbidden"));

            var result = await _controller.GetById(10);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<FlightScheduleDetailsDto>>(objectResult.Value);
        }
    }
}
