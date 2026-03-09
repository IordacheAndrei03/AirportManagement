using System.ComponentModel.DataAnnotations;

namespace AirportManagement.Application.Dtos.FlightSchedulesDtos
{
    public class FlightScheduleCreateDto
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        public DateTime ScheduledDepartureUtc { get; set; }

        [Required]
        public DateTime ScheduledArrivalUtc { get; set; }

        [Required]
        public int GateId { get; set; }

        [Required]
        public int AssignedAircraftId { get; set; }
    }
}
