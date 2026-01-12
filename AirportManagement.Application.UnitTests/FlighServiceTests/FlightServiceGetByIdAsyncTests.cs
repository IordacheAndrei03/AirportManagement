using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighServiceBaseTests
{
    public class FlightServiceGetByIdAsyncTests : FlightServiceTestBase
    {
        [Fact]
        public async Task GetByIdAsync_WhenFlightNotFound_ReturnsNotFound()
        {
            _flightRepository
             .Setup(x => x.GetByIdWithDetailsAsync(10))
             .Returns(Task.FromResult<Flight?>(null));

            var result = await _flightService.GetByIdAsync(10);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFlightFound_ReturnsSuccessWithMappedDto()
        {
            var flight = new Flight { Id = 10 };
            var dto = new FlightDetailsDto();

            _flightRepository
                .Setup(x => x.GetByIdWithDetailsAsync(10))
                .Returns(Task.FromResult<Flight?>(flight));

            _mapper
                .Setup(x => x.Map<FlightDetailsDto>(flight))
                .Returns(dto);

            var result = await _flightService.GetByIdAsync(10);

            Assert.True(result.IsSuccess);
            Assert.Same(dto, result.Value);
        }
    }
}
