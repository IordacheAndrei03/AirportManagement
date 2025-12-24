using System;
using System.Collections.Generic;
using System.Linq;

namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class ScheduleImportResultDto
    {
        public int Total { get; set; }

        public int Created { get; set; }

        public int Updated { get; set; }

        public List<ScheduleImportErrorDto> Errors { get; set; } = new();
    }
}
