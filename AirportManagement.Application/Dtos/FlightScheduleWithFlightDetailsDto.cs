using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos
{
    public class FlightSearchScheduleDto
    {
        public string AirlineIata { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;

        public string OriginIata { get; set; } = string.Empty;
        public string DestinationIata { get; set; } = string.Empty;

        public DateTime ScheduledDepartureUtc { get; set; }
        public DateTime ScheduledArrivalUtc { get; set; }

        public string GateCode { get; set; } = string.Empty;      
        public string AircraftTail { get; set; } = string.Empty; 
    }
}
