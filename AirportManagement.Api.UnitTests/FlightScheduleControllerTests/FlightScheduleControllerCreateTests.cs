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
    public class FlightScheduleControllerCreateTests : FlightScheduleControllerTestBase
    {
        [Fact]
        public async Task Create_WhenOk_Returns201_WithResultObjectInBody()
        {
            var dto = CreateValidFlightScheduleCreateDto();

            _scheduleService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<int>.Success(123));

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);

            var returned = Assert.IsType<ResultObject<int>>(objectResult.Value);
            Assert.True(returned.IsSuccess);
            Assert.Equal(123, returned.Value);

            _scheduleService.Verify(s => s.CreateAsync(dto), Times.Once);
        }

        [Fact]
        public async Task Create_WhenInvalid_Returns400()
        {
            var dto = CreateValidFlightScheduleCreateDto();

            _scheduleService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<int>.Invalid("invalid"));

            var result = await _controller.Create(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<ResultObject<int>>(badRequest.Value);
        }

        [Fact]
        public async Task Create_WhenNotFound_Returns404()
        {
            var dto = CreateValidFlightScheduleCreateDto();

            _scheduleService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<int>.NotFound("not found"));

            var result = await _controller.Create(dto);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.IsType<ResultObject<int>>(notFound.Value);
        }

        [Fact]
        public async Task Create_WhenConflict_Returns409()
        {
            var dto = CreateValidFlightScheduleCreateDto();

            _scheduleService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<int>.Conflict("conflict"));

            var result = await _controller.Create(dto);

            var conflict = Assert.IsType<ConflictObjectResult>(result);
            Assert.IsType<ResultObject<int>>(conflict.Value);
        }

        [Fact]
        public async Task Create_WhenForbidden_Returns403()
        {
            var dto = CreateValidFlightScheduleCreateDto();

            _scheduleService.Setup(s => s.CreateAsync(dto))
                .ReturnsAsync(ResultObject<int>.Forbidden("forbidden"));

            var result = await _controller.Create(dto);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<int>>(objectResult.Value);
        }
    }
}
