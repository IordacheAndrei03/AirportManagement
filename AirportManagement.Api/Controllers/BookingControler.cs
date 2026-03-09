using AirportManagement.Api.Extensions;
using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AirportManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        [Authorize(Roles = "Staff,Client")]
        public async Task<ActionResult<BookingCreateResponseDto>> Create([FromBody] BookingCreateRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _bookingService.CreateAsync();

            return this.ToCreatedResult(result);
        }

        [HttpGet("{code}")]
        [Authorize(Roles = "Staff,Client")]
        public async Task<ActionResult<BookingDetailsDto>> GetByCode(string code)
        {
            var result = await _bookingService.GetByCodeAsync(code);

            return this.ToActionResult(result);
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = "Staff,Client")]
        public async Task<ActionResult> Cancel(string code)
        {
            var result = await _bookingService.CancelAsync(code);

            return this.ToActionResult(result);
        }
    }
}

