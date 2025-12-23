using AirportManagement.Application.Dtos.Flights;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightsController : ControllerBase
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Client,Staff")]
        public async Task<ActionResult<FlightDetailsDto>> GetById(
            int id)
        {

            var result = await _flightService.GetByIdAsync(id);

            return result.Status switch
            {
                ResultStatus.Ok => Ok(result.Value),
                ResultStatus.NotFound => NotFound(new ProblemDetails
                {
                    Title = "Flight not found",
                    Detail = result.Error
                }),
                _ => Problem(statusCode: 500, title: "Unexpected error")
            };

        }

        [HttpGet]
        [Authorize(Roles = "Client,Staff")]
        public async Task<ActionResult<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDate(
            [FromQuery] string origin,
            [FromQuery] string destination,
            [FromQuery] DateOnly date,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _flightService.SearchByRouteAndDateAsync(
                origin,
                destination,
                date,
                page,
                pageSize);

            return result.Status switch
            {
                ResultStatus.Ok => Ok(result.Value),
                ResultStatus.Invalid => BadRequest(new ValidationProblemDetails
                {
                    Title = "Invalid flight search",
                    Detail = result.Error
                }),
                _ => Problem(statusCode: 500, title: "Unexpected error")
            };
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<int>> Create(
           [FromBody] FlightCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var id = await _flightService.CreateFlightAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                id);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Update(
            int id,
            [FromBody] FlightCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            await _flightService.UpdateAsync(id, dto);
            return Ok();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Delete(int id)
        {
            await _flightService.DeleteAsync(id);
            return NoContent();
        }

    }
}
