using AirportManagement.Api.Extensions;
using AirportManagement.Application;
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
        public async Task<ActionResult<FlightDetailsDto>> GetById(int id)
        {
            var result = await _flightService.GetByIdAsync(id);

            return this.ToActionResult(result);
        }

        [HttpGet]
        [Authorize(Roles = "Client,Staff")]
        public async Task<ActionResult<IReadOnlyList<FlightSearchScheduleDto>>> SearchByRouteAndDate([FromQuery] string origin, [FromQuery] string destination, [FromQuery] DateOnly date, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _flightService.SearchByRouteAndDateAsync(origin, destination, date, page, pageSize);

            return this.ToActionResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<int>> Create([FromBody] FlightCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _flightService.CreateFlightAsync(dto);

            return this.ToCreatedResult(result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Update(int id, [FromBody] FlightCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _flightService.UpdateAsync(id, dto);

            return this.ToActionResult(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _flightService.DeleteAsync(id);

            return this.ToActionResult(result);
        }
    }
}
