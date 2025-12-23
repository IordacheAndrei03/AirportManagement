using AirportManagement.Application.Dtos.BookingDtos;
using AirportManagement.Application.Interfaces.ServiceInterfaces;
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
        public async Task<ActionResult<BookingCreateResponseDto>> Create(
            [FromBody] BookingCreateRequestDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var result = await _bookingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetByCode), new { code = result.ConfirmationCode }, result);
        }

        [HttpGet("{code}")]
        public async Task<ActionResult<BookingDetailsDto>> GetByCode(
            string code)
        {
            var dto = await _bookingService.GetByCodeAsync(code);
            if (dto is null)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Booking not found",
                    Detail = $"Booking with code '{code}' not found."
                });
            }

            return Ok(dto);
        }

        [HttpDelete("{code}")]
        public async Task<ActionResult> Cancel(string code)
        {
            await _bookingService.CancelAsync(code);
            return NoContent();
        }
    }
}

