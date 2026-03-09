
namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class FlightScheduleDetailsDto
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        public string AirlineIata { get; set; } = string.Empty;

        public string FlightNumber { get; set; } = string.Empty;

        public string OriginIata { get; set; } = string.Empty;

        public string DestinationIata { get; set; } = string.Empty;

        public DateTime ScheduledDepartureUtc { get; set; }

        public DateTime ScheduledArrivalUtc { get; set; }

        public string GateCode { get; set; } = string.Empty;    

        public string AircraftTail { get; set; } = string.Empty; 

        public string Status { get; set; } = string.Empty;   
    }
}
