using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighServiceBaseTests
{
    public class FlightServiceSearchByRouteAndDateAsyncTests : FlightServiceTestBase
    {
        [Fact]
        public async Task SearchByRouteAndDateAsync_WhenOriginMissing_ReturnsInvalid()
        {
            var result = await _flightService.SearchByRouteAndDateAsync(
                originIata: "",
                destinationIata: "LHR",
                departureDate: CreateValidDepartureDate(),
                page: 1,
                pageSize: 20);

            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SearchByRouteAndDateAsync_WhenDestinationMissing_ReturnsInvalid()
        {
            var result = await _flightService.SearchByRouteAndDateAsync(
                originIata: "OTP",
                destinationIata: " ",
                departureDate: CreateValidDepartureDate(),
                page: 1,
                pageSize: 20);

            Assert.False(result.IsSuccess);
            Assert.Contains("required", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SearchByRouteAndDateAsync_WhenOriginEqualsDestination_ReturnsInvalid()
        {
            var result = await _flightService.SearchByRouteAndDateAsync(
                originIata: "OTP",
                destinationIata: "otp",
                departureDate: CreateValidDepartureDate(),
                page: 1,
                pageSize: 20);

            Assert.False(result.IsSuccess);
            Assert.Contains("must be different", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task SearchByRouteAndDateAsync_WhenPageAndPageSizeAreZero_UsesDefaults()
        {
            var departureDate = CreateValidDepartureDate();
            var expectedDepartureUtc = new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc);
            IReadOnlyList<FlightSchedule> schedules = new List<FlightSchedule>
              {
                  new FlightSchedule { Id = 1 }
              };
            IReadOnlyList<FlightSearchScheduleDto> dtoList = new List<FlightSearchScheduleDto>
              {
                  new FlightSearchScheduleDto()
              };
            _flightScheduleRepository
                .Setup(x => x.SearchUpcomingByRouteAndDateAsync("OTP", "LHR", expectedDepartureUtc, 1, 20))
                .ReturnsAsync(schedules);
            _mapper
                .Setup(x => x.Map<IReadOnlyList<FlightSearchScheduleDto>>(schedules))
                .Returns(dtoList);

            var result = await _flightService.SearchByRouteAndDateAsync("OTP", "LHR", departureDate, page: 0, pageSize: 0);

            Assert.True(result.IsSuccess);
            Assert.Same(dtoList, result.Value);
        }

        [Fact]
        public async Task SearchByRouteAndDateAsync_WhenPageSizeTooLarge_ClampsTo100()
        {
            var departureDate = CreateValidDepartureDate();
            var expectedDepartureUtc = new DateTime(2026, 1, 9, 0, 0, 0, DateTimeKind.Utc);
            IReadOnlyList<FlightSchedule> schedules = new List<FlightSchedule>();
            IReadOnlyList<FlightSearchScheduleDto> dtoList = new List<FlightSearchScheduleDto>();
            _flightScheduleRepository
                .Setup(x => x.SearchUpcomingByRouteAndDateAsync("OTP", "LHR", expectedDepartureUtc, 2, 100))
                .ReturnsAsync(schedules);
            _mapper
                .Setup(x => x.Map<IReadOnlyList<FlightSearchScheduleDto>>(schedules))
                .Returns(dtoList);

            var result = await _flightService.SearchByRouteAndDateAsync("OTP", "LHR", departureDate, page: 2, pageSize: 999);

            Assert.True(result.IsSuccess);
            Assert.Same(dtoList, result.Value);
        }
    }
}
