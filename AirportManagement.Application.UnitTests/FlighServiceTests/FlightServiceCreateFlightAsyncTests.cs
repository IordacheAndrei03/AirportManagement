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
    public class FlightServiceCreateFlightAsyncTests : FlightServiceTestBase
    {
        [Fact]
        public async Task CreateFlightAsync_WhenOriginEqualsDestination_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            dto.DestinationIata = dto.OriginIata.ToLowerInvariant();

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("must be different", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenAirlineUnknown_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            _airlineRepository
                .Setup(x => x.GetByIataCodeAsync(dto.AirlineIata))
                .ReturnsAsync((Airline?)null);
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.OriginIata)).ReturnsAsync(new Airport { Id = 10 });
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.DestinationIata)).ReturnsAsync(new Airport { Id = 11 });
            _aircraftRepository.Setup(x => x.GetByTailNoAsync(dto.DefaultAircraftTail)).ReturnsAsync(new Aircraft { Id = 100 });

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Unknown airline", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenOriginAirportUnknown_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            _airlineRepository.Setup(x => x.GetByIataCodeAsync(dto.AirlineIata)).ReturnsAsync(new Airline { Id = 1 });
            _airportRepository
                .Setup(x => x.GetByIataCodeAsync(dto.OriginIata))
                .ReturnsAsync((Airport?)null);
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.DestinationIata)).ReturnsAsync(new Airport { Id = 11 });
            _aircraftRepository.Setup(x => x.GetByTailNoAsync(dto.DefaultAircraftTail)).ReturnsAsync(new Aircraft { Id = 100 });

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Unknown origin airport", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenDestinationAirportUnknown_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            _airlineRepository.Setup(x => x.GetByIataCodeAsync(dto.AirlineIata)).ReturnsAsync(new Airline { Id = 1 });
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.OriginIata)).ReturnsAsync(new Airport { Id = 10 });
            _airportRepository
                .Setup(x => x.GetByIataCodeAsync(dto.DestinationIata))
                .ReturnsAsync((Airport?)null);
            _aircraftRepository.Setup(x => x.GetByTailNoAsync(dto.DefaultAircraftTail)).ReturnsAsync(new Aircraft { Id = 100 });

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Unknown destination airport", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenAircraftUnknown_ReturnsInvalid()
        {
            var dto = CreateValidFlightCreateDto();
            _airlineRepository.Setup(x => x.GetByIataCodeAsync(dto.AirlineIata)).ReturnsAsync(new Airline { Id = 1 });
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.OriginIata)).ReturnsAsync(new Airport { Id = 10 });
            _airportRepository.Setup(x => x.GetByIataCodeAsync(dto.DestinationIata)).ReturnsAsync(new Airport { Id = 11 });
            _aircraftRepository
                .Setup(x => x.GetByTailNoAsync(dto.DefaultAircraftTail))
                .ReturnsAsync((Aircraft?)null);

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("Unknown aircraft", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenDuplicateRouteExists_ReturnsConflict()
        {
            var dto = CreateValidFlightCreateDto();
            ArrangeAllLookupsValid(dto, airlineId: 1, originId: 10, destinationId: 11, aircraftId: 100);
            _flightRepository
                .Setup(x => x.ExistsDuplicateRouteAsync(1, dto.FlightNumber, 10, 11, null))
                .ReturnsAsync(true);

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("already exists", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenMapperMakesAirportsEqual_ReturnsConflict()
        {
            var dto = CreateValidFlightCreateDto();
            ArrangeAllLookupsValid(dto, airlineId: 1, originId: 10, destinationId: 11, aircraftId: 100);
            _flightRepository
                .Setup(x => x.ExistsDuplicateRouteAsync(1, dto.FlightNumber, 10, 11, null))
                .ReturnsAsync(false);
            _mapper
                .Setup(x => x.Map<Flight>(It.IsAny<Flight>()))
                .Returns<Flight>(f =>
                {
                    f.OriginAirport = 10;
                    f.DestinationAirport = 10;
                    return f;
                });

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.Contains("are equal", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateFlightAsync_WhenValid_AddsFlightAndReturnsId()
        {
            var dto = CreateValidFlightCreateDto();
            ArrangeAllLookupsValid(dto, airlineId: 1, originId: 10, destinationId: 11, aircraftId: 100);
            _flightRepository
                .Setup(x => x.ExistsDuplicateRouteAsync(1, dto.FlightNumber, 10, 11, null))
                .ReturnsAsync(false);
            _mapper
                .Setup(x => x.Map<Flight>(It.IsAny<Flight>()))
                .Returns<Flight>(f => f);
            CaptureAddFlight(forcedId: 999);

            var result = await _flightService.CreateFlightAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(999, result.Value);
            Assert.NotNull(_capturedAddedFlight);
            Assert.Equal(1, _capturedAddedFlight!.AirlineId);
            Assert.Equal(dto.FlightNumber, _capturedAddedFlight.FlightNumber);
            Assert.Equal(10, _capturedAddedFlight.OriginAirport);
            Assert.Equal(11, _capturedAddedFlight.DestinationAirport);
            Assert.Equal(100, _capturedAddedFlight.DefaultAircraftId);
            Assert.Equal(dto.IsActive, _capturedAddedFlight.IsActive);
        }
    }
}
