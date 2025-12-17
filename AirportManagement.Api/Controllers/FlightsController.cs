using AirportManagement.Application.Dtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Domain.Entities;
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
        public async Task<ActionResult<FlightDetailsDto>> GetById(int id)
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

    }
}
