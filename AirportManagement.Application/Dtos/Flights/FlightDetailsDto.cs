using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos.Flights
{
    public class FlightDetailsDto
    {

        public string AirlineName { get; set; } = string.Empty;

        public string FlightNumber { get; set; } = string.Empty;

        public string OriginIata { get; set; } = string.Empty;

        public string DestinationIata { get; set; } = string.Empty;

        public string DefaultAircraftModel { get; set; } = string.Empty;

        public bool IsActive { get; set; }
    }
}
