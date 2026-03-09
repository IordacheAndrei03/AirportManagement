
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighScheduleServiceTests
{
    public class FlightScheduleServiceCreateAsyncTests : FlightScheduleServiceTestBase
    {
        [Fact]
        public async Task CreateAsync_WhenArrivalBeforeDeparture_ReturnsInvalid()
        {
            var dto = CreateValidCreateDto();
            dto.ScheduledArrivalUtc = dto.ScheduledDepartureUtc; 

            var result = await _flightScheduleService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("Arrival must be after departure", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenGateOverlap_ReturnsConflict()
        {
            var dto = CreateValidCreateDto();
            ArrangeGateOverlap(dto);

            var result = await _flightScheduleService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("Gate is already used", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenStatusNotFound_ReturnsNotFound()
        {
            var dto = CreateValidCreateDto();
            ArrangeNoGateOverlap(dto);
            ArrangeScheduledStatusId(0);

            var result = await _flightScheduleService.CreateAsync(dto);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("Scheduled", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task CreateAsync_WhenValid_AddsEntityAndReturnsId()
        {
            var dto = CreateValidCreateDto();
            ArrangeNoGateOverlap(dto);
            ArrangeScheduledStatusId(12);
            ArrangeCaptureAddAsync(forcedId: 999);

            var result = await _flightScheduleService.CreateAsync(dto);

            Assert.True(result.IsSuccess);
            Assert.Equal(999, result.Value);
            Assert.NotNull(_capturedFlightSchedule);
            Assert.Equal(dto.FlightId, _capturedFlightSchedule!.FlightId);
            Assert.Equal(dto.ScheduledDepartureUtc, _capturedFlightSchedule.ScheduledDepartureUtc);
            Assert.Equal(dto.ScheduledArrivalUtc, _capturedFlightSchedule.ScheduleArrivalUtc);
            Assert.Equal(dto.GateId, _capturedFlightSchedule.GateId);
            Assert.Equal(dto.AssignedAircraftId, _capturedFlightSchedule.AssignedAircraftId);
            Assert.Equal(12, _capturedFlightSchedule.FlightStatusId);
        }
    }
}
