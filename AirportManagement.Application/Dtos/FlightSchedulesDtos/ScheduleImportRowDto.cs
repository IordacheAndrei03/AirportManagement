
namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class ScheduleImportRowDto
    {
        public string FlightNumber { get; set; } = string.Empty;

        public string AirlineIata { get; set; } = string.Empty;

        public string OriginIata { get; set; } = string.Empty;

        public string DestinationIata { get; set; } = string.Empty;

        public DateTime ScheduledDepartureUtc { get; set; }

        public DateTime ScheduledArrivalUtc { get; set; }

        public string GateCode { get; set; } = string.Empty;

        public string AssignedAircraftTail { get; set; } = string.Empty;
    }
}
