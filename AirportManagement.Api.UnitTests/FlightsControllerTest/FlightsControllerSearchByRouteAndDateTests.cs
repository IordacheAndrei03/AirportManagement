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
    public class FlightsControllerSearchByRouteAndDateTests : FlightsControllerTestBase
    {
        [Fact]
        public async Task SearchByRouteAndDate_WhenOk_Returns200_WithResultObjectInBody_AndCallsServiceWithSameArgs()
        {
            var date = new DateOnly(2026, 1, 9);
            IReadOnlyList<FlightSearchScheduleDto> list = new List<FlightSearchScheduleDto>
        {
            new FlightSearchScheduleDto()
        };

            var serviceResult = ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Success(list);

            _flightService.Setup(s => s.SearchByRouteAndDateAsync("OTP", "LHR", date, 2, 50))
                .ReturnsAsync(serviceResult);

            var result = await _controller.SearchByRouteAndDate("OTP", "LHR", date, page: 2, pageSize: 50);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>>(ok.Value);

            Assert.True(returned.IsSuccess);
            Assert.Single(returned.Value!);

            _flightService.Verify(s => s.SearchByRouteAndDateAsync("OTP", "LHR", date, 2, 50), Times.Once);
        }

        [Fact]
        public async Task SearchByRouteAndDate_WhenInvalid_Returns400()
        {
            var date = new DateOnly(2026, 1, 9);

            _flightService.Setup(s => s.SearchByRouteAndDateAsync("OTP", "OTP", date, 1, 20))
                .ReturnsAsync(ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Invalid("invalid"));

            var result = await _controller.SearchByRouteAndDate("OTP", "OTP", date);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>>(badRequest.Value);
        }

        [Fact]
        public async Task SearchByRouteAndDate_WhenNotFound_Returns404()
        {
            var date = new DateOnly(2026, 1, 9);

            _flightService.Setup(s => s.SearchByRouteAndDateAsync("OTP", "LHR", date, 1, 20))
                .ReturnsAsync(ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.NotFound("not found"));

            var result = await _controller.SearchByRouteAndDate("OTP", "LHR", date);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>>(notFound.Value);
        }

        [Fact]
        public async Task SearchByRouteAndDate_WhenConflict_Returns409()
        {
            var date = new DateOnly(2026, 1, 9);

            _flightService.Setup(s => s.SearchByRouteAndDateAsync("OTP", "LHR", date, 1, 20))
                .ReturnsAsync(ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Conflict("conflict"));

            var result = await _controller.SearchByRouteAndDate("OTP", "LHR", date);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>>(conflict.Value);
        }

        [Fact]
        public async Task SearchByRouteAndDate_WhenForbidden_Returns403()
        {
            var date = new DateOnly(2026, 1, 9);

            _flightService.Setup(s => s.SearchByRouteAndDateAsync("OTP", "LHR", date, 1, 20))
                .ReturnsAsync(ResultObject<IReadOnlyList<FlightSearchScheduleDto>>.Forbidden("forbidden"));

            var result = await _controller.SearchByRouteAndDate("OTP", "LHR", date);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<IReadOnlyList<FlightSearchScheduleDto>>>(objectResult.Value);
        }
    }
}
