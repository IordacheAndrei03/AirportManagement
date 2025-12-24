
namespace AirportManagement.Domain.Entities
{
    public class FlightStatus
    {
        public int Id { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}
