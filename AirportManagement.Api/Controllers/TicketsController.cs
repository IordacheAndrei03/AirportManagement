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
        public async Task<ActionResult<ResultObject<TicketCreateResponseDto>>> Create([FromBody] TicketCreateRequestDto dto)
        {
            var result = await _ticketService.CreateAsync(dto);

            return result.Status switch
            {
                ResultStatus.Ok => StatusCode(StatusCodes.Status201Created, result),
                ResultStatus.NotFound => NotFound(result),
                ResultStatus.Invalid => BadRequest(result),
                _ => BadRequest(result)
            };
        }

        [HttpGet("by-flight/{flightScheduleId:int}")]
        public async Task<ActionResult<IReadOnlyList<TicketByFlightScheduleDto>>> GetByFlightSchedule(int flightScheduleId)
        {
            try
            {
                var result = await _ticketService.GetByFlightScheduleAsync(flightScheduleId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Invalid request",
                    Detail = ex.Message
                });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Delete(int id)
        {
            await _ticketService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{ticketId:int}/seat")]
        public async Task<ActionResult<ResultObject<TicketSeatUpdateDto>>> UpdateSeat(
        int ticketId,
        [FromBody] TicketSeatUpdateDto body)
        {
            var result = await _ticketService.UpdateSeatNumberAsync(ticketId, body.SeatNumber);

            return result.Status switch
            {
                ResultStatus.Ok => Ok(result),
                ResultStatus.NotFound => NotFound(result),
                ResultStatus.Invalid => BadRequest(result),
                _ => BadRequest(result)
            };
        }
    }
}
