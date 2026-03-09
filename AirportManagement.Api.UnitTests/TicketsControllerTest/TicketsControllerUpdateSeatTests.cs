using AirportManagement.Application;
using AirportManagement.Application.Dtos.TicketDtos;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AirportManagement.Api.UnitTests.TicketsControllerTest
{
    public class TicketsControllerUpdateSeatTests : TicketsControllerTestBase
    {
        [Fact]
        public async Task UpdateSeat_WhenOk_Returns200_WithResultObjectInBody_AndCallsServiceWithSeatNumber()
        {
            var body = new TicketSeatUpdateDto
            {
                TicketId = 10,
                SeatNumber = "12B"
            };

            var responseDto = new TicketSeatUpdateDto
            {
                TicketId = 10,
                SeatNumber = "12B"
            };

            _ticketService.Setup(s => s.UpdateSeatNumberAsync(10, "12B"))
                .ReturnsAsync(ResultObject<TicketSeatUpdateDto>.Success(responseDto));

            var result = await _controller.UpdateSeat(10, body);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<TicketSeatUpdateDto>>(ok.Value);

            Assert.True(returned.IsSuccess);
            Assert.Equal("12B", returned.Value!.SeatNumber);

            _ticketService.Verify(s => s.UpdateSeatNumberAsync(10, "12B"), Times.Once);
        }

        [Fact]
        public async Task UpdateSeat_WhenInvalid_Returns400()
        {
            var body = new TicketSeatUpdateDto { SeatNumber = " " };

            _ticketService.Setup(s => s.UpdateSeatNumberAsync(10, " "))
                .ReturnsAsync(ResultObject<TicketSeatUpdateDto>.Invalid("invalid"));

            var result = await _controller.UpdateSeat(10, body);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketSeatUpdateDto>>(badRequest.Value);
        }

        [Fact]
        public async Task UpdateSeat_WhenNotFound_Returns404()
        {
            var body = new TicketSeatUpdateDto { SeatNumber = "12B" };

            _ticketService.Setup(s => s.UpdateSeatNumberAsync(10, "12B"))
                .ReturnsAsync(ResultObject<TicketSeatUpdateDto>.NotFound("not found"));

            var result = await _controller.UpdateSeat(10, body);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketSeatUpdateDto>>(notFound.Value);
        }

        [Fact]
        public async Task UpdateSeat_WhenConflict_Returns409()
        {
            var body = new TicketSeatUpdateDto { SeatNumber = "12B" };

            _ticketService.Setup(s => s.UpdateSeatNumberAsync(10, "12B"))
                .ReturnsAsync(ResultObject<TicketSeatUpdateDto>.Conflict("conflict"));

            var result = await _controller.UpdateSeat(10, body);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<TicketSeatUpdateDto>>(conflict.Value);
        }

        [Fact]
        public async Task UpdateSeat_WhenForbidden_Returns403()
        {
            var body = new TicketSeatUpdateDto { SeatNumber = "12B" };

            _ticketService.Setup(s => s.UpdateSeatNumberAsync(10, "12B"))
                .ReturnsAsync(ResultObject<TicketSeatUpdateDto>.Forbidden("forbidden"));

            var result = await _controller.UpdateSeat(10, body);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<TicketSeatUpdateDto>>(objectResult.Value);
        }
    }
}
