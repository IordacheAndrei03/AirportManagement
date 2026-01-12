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
    public class FlightScheduleControllerGetUpcomingStatsTests : FlightScheduleControllerTestBase
    {
        [Fact]
        public async Task GetUpcomingStats_WhenOk_Returns200_WithResultObjectInBody()
        {
            IReadOnlyList<UpcomingSchedulesDto> list = new List<UpcomingSchedulesDto>
        {
            new UpcomingSchedulesDto { Date = DateTime.UtcNow.Date, Flights = 3 }
        };

            _scheduleService.Setup(s => s.GetUpcomingStatsAsync(7))
                .ReturnsAsync(ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Success(list));

            var result = await _controller.GetUpcomingStats(days: 7);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>>(ok.Value);
            Assert.True(returned.IsSuccess);
            Assert.Single(returned.Value!);

            _scheduleService.Verify(s => s.GetUpcomingStatsAsync(7), Times.Once);
        }

        [Fact]
        public async Task GetUpcomingStats_WhenInvalid_Returns400()
        {
            _scheduleService.Setup(s => s.GetUpcomingStatsAsync(0))
                .ReturnsAsync(ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Invalid("invalid"));

            var result = await _controller.GetUpcomingStats(days: 0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>>(badRequest.Value);
        }

        [Fact]
        public async Task GetUpcomingStats_WhenNotFound_Returns404()
        {
            _scheduleService.Setup(s => s.GetUpcomingStatsAsync(7))
                .ReturnsAsync(ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.NotFound("not found"));

            var result = await _controller.GetUpcomingStats(days: 7);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>>(notFound.Value);
        }

        [Fact]
        public async Task GetUpcomingStats_WhenConflict_Returns409()
        {
            _scheduleService.Setup(s => s.GetUpcomingStatsAsync(7))
                .ReturnsAsync(ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Conflict("conflict"));

            var result = await _controller.GetUpcomingStats(days: 7);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>>(conflict.Value);
        }

        [Fact]
        public async Task GetUpcomingStats_WhenForbidden_Returns403()
        {
            _scheduleService.Setup(s => s.GetUpcomingStatsAsync(7))
                .ReturnsAsync(ResultObject<IReadOnlyList<UpcomingSchedulesDto>>.Forbidden("forbidden"));

            var result = await _controller.GetUpcomingStats(days: 7);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<IReadOnlyList<UpcomingSchedulesDto>>>(objectResult.Value);
        }
    }
}
