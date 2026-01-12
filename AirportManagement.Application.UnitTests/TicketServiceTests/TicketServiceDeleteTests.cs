using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace AirportManagement.Application.UnitTests.TicketServiceTests
{
    public class TicketServiceDeleteTests : TicketServiceTestBase
    {
        [Fact]
        public async Task DeleteAsync_WhenTicketNotFound_ReturnsNotFound()
        {
            _ticketRepository
                .Setup(x => x.GetByIdAsync(10))
                .Returns(Task.FromResult<Ticket?>(null));

            var result = await _ticketService.DeleteAsync(10);

            Assert.False(result.IsSuccess);
            Assert.Contains("not found", result.Error!, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task DeleteAsync_WhenTicketFound_DeletesAndReturnsSuccess()
        {
            _ticketRepository
                .Setup(x => x.GetByIdAsync(10))
                .Returns(Task.FromResult<Ticket?>(new Ticket { Id = 10 }));

            CaptureDeleteTicketId();

            var result = await _ticketService.DeleteAsync(10);

            Assert.True(result.IsSuccess);
            Assert.Equal(10, _capturedDeletedTicketId);
        }
    }
}
