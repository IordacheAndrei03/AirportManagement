using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Application.UnitTests.TicketServiceTests
{
    public class TicketServiceGetByFlightScheduleTests : TicketServiceTestBase
    {
        [Fact]
        public async Task GetByFlightScheduleAsync_WhenIdNotPositive_ReturnsInvalid()
        {
            var result = await _ticketService.GetByFlightScheduleAsync(0);

            Assert.False(result.IsSuccess);
            Assert.Contains("must be positive", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task GetByFlightScheduleAsync_WhenValid_ReturnsMappedList()
        {
            var tickets = new List<Ticket>
        {
            new Ticket { Id = 1, SeatNumber = "1A" },
            new Ticket { Id = 2, SeatNumber = "2B" }
        };

            _ticketRepository
                .Setup(x => x.GetByFlightScheduleAsync(20))
                .ReturnsAsync(tickets);

            var dto1 = new TicketByFlightScheduleDto();
            var dto2 = new TicketByFlightScheduleDto();

            _mapper.Setup(x => x.Map<TicketByFlightScheduleDto>(tickets[0])).Returns(dto1);
            _mapper.Setup(x => x.Map<TicketByFlightScheduleDto>(tickets[1])).Returns(dto2);

            var result = await _ticketService.GetByFlightScheduleAsync(20);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value!.Count);
            Assert.Same(dto1, result.Value[0]);
            Assert.Same(dto2, result.Value[1]);
        }
    }
}
