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
    public class TicketsControllerGetByFlightScheduleTests : TicketsControllerTestBase
    {
        [Fact]
        public async Task GetByFlightSchedule_WhenOk_Returns200_WithResultObjectInBody()
        {
            IReadOnlyList<TicketByFlightScheduleDto> list = new List<TicketByFlightScheduleDto>
        {
            new TicketByFlightScheduleDto(),
            new TicketByFlightScheduleDto()
        };

            _ticketService.Setup(s => s.GetByFlightScheduleAsync(20))
                .ReturnsAsync(ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Success(list));

            var result = await _controller.GetByFlightSchedule(20);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>>(ok.Value);

            Assert.True(returned.IsSuccess);
            Assert.Equal(2, returned.Value!.Count);

            _ticketService.Verify(s => s.GetByFlightScheduleAsync(20), Times.Once);
        }

        [Fact]
        public async Task GetByFlightSchedule_WhenInvalid_Returns400()
        {
            _ticketService.Setup(s => s.GetByFlightScheduleAsync(0))
                .ReturnsAsync(ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Invalid("invalid"));

            var result = await _controller.GetByFlightSchedule(0);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>>(badRequest.Value);
        }

        [Fact]
        public async Task GetByFlightSchedule_WhenNotFound_Returns404()
        {
            _ticketService.Setup(s => s.GetByFlightScheduleAsync(20))
                .ReturnsAsync(ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.NotFound("not found"));

            var result = await _controller.GetByFlightSchedule(20);

            var notFound = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>>(notFound.Value);
        }

        [Fact]
        public async Task GetByFlightSchedule_WhenConflict_Returns409()
        {
            _ticketService.Setup(s => s.GetByFlightScheduleAsync(20))
                .ReturnsAsync(ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Conflict("conflict"));

            var result = await _controller.GetByFlightSchedule(20);

            var conflict = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.IsType<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>>(conflict.Value);
        }

        [Fact]
        public async Task GetByFlightSchedule_WhenForbidden_Returns403()
        {
            _ticketService.Setup(s => s.GetByFlightScheduleAsync(20))
                .ReturnsAsync(ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>.Forbidden("forbidden"));

            var result = await _controller.GetByFlightSchedule(20);

            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(403, objectResult.StatusCode);
            Assert.IsType<ResultObject<IReadOnlyList<TicketByFlightScheduleDto>>>(objectResult.Value);
        }
    }
}
