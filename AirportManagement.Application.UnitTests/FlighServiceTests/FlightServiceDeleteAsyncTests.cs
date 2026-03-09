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
    public class FlightServiceDeleteAsyncTests : FlightServiceTestBase
    {
        [Fact]
        public async Task DeleteAsync_WhenFlightNotFound_ReturnsNotFound()
        {
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((Flight?)null);

            var result = await _flightService.DeleteAsync(10);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteAsync_WhenHasSchedules_ReturnsConflict()
        {
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Flight { Id = 10 });

            _flightScheduleRepository.Setup(x => x.AnyByFlightIdAsync(10)).ReturnsAsync(true);

            var result = await _flightService.DeleteAsync(10);

            Assert.False(result.IsSuccess);
            Assert.Contains("cannot be deleted", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteAsync_WhenNoSchedules_DeletesAndReturnsSuccess()
        {
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(new Flight { Id = 10 });

            _flightScheduleRepository.Setup(x => x.AnyByFlightIdAsync(10)).ReturnsAsync(false);

            CaptureDeleteFlightId();

            var result = await _flightService.DeleteAsync(10);

            Assert.True(result.IsSuccess);
            Assert.Equal(10, _capturedDeletedFlightId);
        }
    }
}
