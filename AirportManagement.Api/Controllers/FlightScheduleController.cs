using AirportManagement.Api.Extensions;
using AirportManagement.Application.Dtos.FlightSchedulesDtos;
using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Client,Staff")]
        public async Task<ActionResult<FlightScheduleDetailsDto>> GetById(int id)
        {
            var result = await _scheduleService.GetByIdAsync(id);

            return this.ToActionResult(result);
        }

        [HttpGet("upcoming-stats/{days:int}")]
        [Authorize(Roles = "Client,Staff")]
        public async Task<ActionResult<IReadOnlyList<UpcomingSchedulesDto>>> GetUpcomingStats([FromRoute] int days = 7)
        {
            var result = await _scheduleService.GetUpcomingStatsAsync(days);

            return this.ToActionResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Create([FromBody] FlightScheduleCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _scheduleService.CreateAsync(dto);

            return this.ToCreatedResult(result);
        }

        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<ScheduleImportResultDto>> Import([FromForm] ScheduleImportRequest request)
        {
            var file = request.File;
            var result = await _scheduleService.ImportAsync(file);

            return result.Status == ResultStatus.Ok && result.Value!.Errors.Count == 0
                ? StatusCode(StatusCodes.Status201Created, result)
                : StatusCode(StatusCodes.Status207MultiStatus, result);
        }
    }
}

