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
    public class FlightServiceUpdateAsyncTests : FlightServiceTestBase
    {
        [Fact]
        public async Task UpdateAsync_WhenOriginEqualsDestination_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            dto.DestinationIata = dto.OriginIata.ToLowerInvariant();

            var result = await _flightService.UpdateAsync(10, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("must be different", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateAsync_WhenFlightNotFound_ReturnsNotFound()
        {
            var dto = CreateValidFlightCreateDto();
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync((Flight?)null);

            var result = await _flightService.UpdateAsync(10, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateAsync_WhenDuplicateRouteExists_ReturnsConflict()
        {
            var dto = CreateValidFlightCreateDto();
            var flight = CreateExistingFlight(10);
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(flight);
            ArrangeAllLookupsValid(dto, airlineId: 1, originId: 10, destinationId: 11, aircraftId: 100);
            _flightRepository
                .Setup(x => x.ExistsDuplicateRouteAsync(1, dto.FlightNumber, 10, 11, 10))
                .ReturnsAsync(true);

            var result = await _flightService.UpdateAsync(10, dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("already exists", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateAsync_WhenValid_UpdatesFlightAndReturnsSuccess()
        {
            var dto = CreateValidFlightCreateDto();
            dto.FlightNumber = "200";
            dto.DefaultAircraftTail = "YR-XYZ";
            dto.IsActive = false;
            var flight = CreateExistingFlight(10);
            _flightRepository.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(flight);
            ArrangeAllLookupsValid(dto, airlineId: 2, originId: 20, destinationId: 21, aircraftId: 200);
            _flightRepository
                .Setup(x => x.ExistsDuplicateRouteAsync(2, "200", 20, 21, 10))
                .ReturnsAsync(false);

            var result = await _flightService.UpdateAsync(10, dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(2, flight.AirlineId);
            Assert.Equal("200", flight.FlightNumber);
            Assert.Equal(20, flight.OriginAirport);
            Assert.Equal(21, flight.DestinationAirport);
            Assert.Equal(200, flight.DefaultAircraftId);
            Assert.False(flight.IsActive);
        }
    }
}
