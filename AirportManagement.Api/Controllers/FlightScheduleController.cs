using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using AirportManagement.Application.Services;
using Microsoft.AspNetCore.Mvc;


namespace AirportManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FlightScheduleController : ControllerBase
    {
        private readonly IFlightScheduleService _scheduleService;

        public FlightScheduleController(IFlightScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<FlightScheduleDetailsDto>> GetById(int id)
        {
            var result = await _scheduleService.GetByIdAsync(id);

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

        [HttpGet("upcoming-stats/{days:int}")]
        public async Task<ActionResult<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStats([FromRoute] int days = 7)
        {
            var result = await _scheduleService.GetUpcomingStatsAsync(days);

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

    }
}
