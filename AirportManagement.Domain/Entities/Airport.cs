
namespace AirportManagement.Domain.Entities
{
    public class Airport
    {
        public int Id { get; set; }

        public string Iatacode { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string TimeZone { get; set; } = null!;

        public int AdressId { get; set; }

        public virtual Adress Adress { get; set; } = null!;

        public virtual ICollection<Flight> FlightDestinationAirportNavigations { get; set; } = new List<Flight>();

        public virtual ICollection<Flight> FlightOriginAirportNavigations { get; set; } = new List<Flight>();

        public virtual ICollection<Gate> Gates { get; set; } = new List<Gate>();
    }

}
