using AirportManagement.Api.Extensions;
using AirportManagement.Application;
using AirportManagement.Application.Dtos.TicketDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Client")]
        public async Task<ActionResult<ResultObject<TicketCreateResponseDto>>> Create([FromBody] TicketCreateRequestDto dto)
        {
            var result = await _ticketService.CreateAsync(dto);

            return this.ToCreatedResult(result);
        }

        [HttpGet("by-flight/{flightScheduleId:int}")]
        [Authorize(Roles = "Staff,Client")]

        public async Task<ActionResult<IReadOnlyList<TicketByFlightScheduleDto>>> GetByFlightSchedule(int flightScheduleId)
        {
            var result = await _ticketService.GetByFlightScheduleAsync(flightScheduleId);

            return this.ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteAsync(id);

            return this.ToActionResult(result);
        }

        [HttpPut("/update-seat")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ResultObject<TicketSeatUpdateDto>>> UpdateSeat(int ticketId, [FromBody] TicketSeatUpdateDto body)
        {
            var result = await _ticketService.UpdateSeatNumberAsync(ticketId, body.SeatNumber);

            return this.ToActionResult(result);
        }
    }
}
