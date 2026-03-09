using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class ScheduleImportRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
