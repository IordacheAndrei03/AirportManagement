using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighScheduleServiceTests
{
    public class FlightScheduleServiceGetUpcomingStatsAsyncTests : FlightScheduleServiceTestBase
    {
        [Fact]
        public async Task GetUpcomingStatsAsync_WhenDaysIsZero_ReturnsInvalid()
        {
            var result = await _flightScheduleService.GetUpcomingStatsAsync(0);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("positive", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetUpcomingStatsAsync_WhenRepoReturnsEmpty_ReturnsNotFound()
        {
            IReadOnlyList<UpcomingSchedulesDto> rows = Array.Empty<UpcomingSchedulesDto>();
            _flightScheduleRepository
                .Setup(x => x.GetUpcomingStatsAsync(5))
                .ReturnsAsync(rows);

            var result = await _flightScheduleService.GetUpcomingStatsAsync(5);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("No upcoming flights", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetUpcomingStatsAsync_WhenRepoReturnsRows_ReturnsSuccess()
        {
            IReadOnlyList<UpcomingSchedulesDto> rows = new List<UpcomingSchedulesDto>
            {
                new UpcomingSchedulesDto { Date = new DateTime(2026, 1, 9), Flights = 5 }
            };

            _flightScheduleRepository
                .Setup(x => x.GetUpcomingStatsAsync(It.IsAny<int>()))
                .ReturnsAsync(rows);

            var result = await _flightScheduleService.GetUpcomingStatsAsync(5);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(rows, result.Value);
            _flightScheduleRepository.Verify(x => x.GetUpcomingStatsAsync(5), Times.Once);
        }
    }
}
