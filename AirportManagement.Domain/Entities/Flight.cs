
namespace AirportManagement.Domain.Entities
{
    public class Flight
    {
        public int Id { get; set; }

        public int AirlineId { get; set; }

        public string FlightNumber { get; set; } = null!;

        public int OriginAirport { get; set; }

        public int DestinationAirport { get; set; }

        public bool IsActive { get; set; }

        public int DefaultAircraftId { get; set; }

        public virtual Airline Airline { get; set; } = null!;

        public virtual Aircraft DefaultAircraft { get; set; } = null!;

        public virtual Airport DestinationAirportNavigation { get; set; } = null!;

        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();

        public virtual Airport OriginAirportNavigation { get; set; } = null!;
    }
}
