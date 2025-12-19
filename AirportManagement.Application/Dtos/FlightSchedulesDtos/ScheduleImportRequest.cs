using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class ScheduleImportRequest
    {
        [Required]
        public IFormFile File { get; set; } = default!;
    }
}
