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
    public class TicketServiceUpdateSeatNumberTests : TicketServiceTestBase
    {
        [Fact]
        public async Task UpdateSeatNumberAsync_WhenTicketIdNotPositive_ReturnsInvalid()
        {
            var result = await _ticketService.UpdateSeatNumberAsync(0, "12A");

            Assert.False(result.IsSuccess);
            Assert.Contains("TicketId", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateSeatNumberAsync_WhenSeatNumberEmpty_ReturnsInvalid()
        {
            var result = await _ticketService.UpdateSeatNumberAsync(10, "   ");

            Assert.False(result.IsSuccess);
            Assert.Contains("SeatNumber", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateSeatNumberAsync_WhenTicketNotFound_ReturnsNotFound()
        {
            _ticketRepository
                .Setup(x => x.GetByIdAsync(10))
                .Returns(Task.FromResult<Ticket?>(null));

            var result = await _ticketService.UpdateSeatNumberAsync(10, "12A");

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task UpdateSeatNumberAsync_WhenValid_UpdatesSeat_AndReturnsSuccess()
        {
            var ticket = new Ticket { Id = 10, SeatNumber = "1A" };

            _ticketRepository
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(ticket);

            var result = await _ticketService.UpdateSeatNumberAsync(10, " 12B ");

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);

            Assert.Equal("12B", ticket.SeatNumber);
            Assert.Equal(10, result.Value!.TicketId);
            Assert.Equal("12B", result.Value.SeatNumber);
        }
    }
}
