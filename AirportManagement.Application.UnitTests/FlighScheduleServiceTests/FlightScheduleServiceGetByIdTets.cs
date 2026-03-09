using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.FlighScheduleServiceTests
{
    public class FlightScheduleServiceGetByIdTets : FlightScheduleServiceTestBase
    {
        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNotFound()
        {
            _flightScheduleRepository
                .Setup(x => x.GetByIdWithDetailsAsync(10))
                .ReturnsAsync((FlightSchedule?)null);

            var result = await _flightScheduleService.GetByIdAsync(10);

            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_ReturnsSuccessWithMappedDto()
        {
            var entity = new FlightSchedule { Id = 10 };
            var dto = new FlightScheduleDetailsDto();

            _flightScheduleRepository
                .Setup(x => x.GetByIdWithDetailsAsync(10))
                .ReturnsAsync(entity);

            _mapper
                .Setup(x => x.Map<FlightScheduleDetailsDto>(entity))
                .Returns(dto);

            var result = await _flightScheduleService.GetByIdAsync(10);

            Assert.True(result.IsSuccess);
            Assert.Same(dto, result.Value);
        }
    }
}
