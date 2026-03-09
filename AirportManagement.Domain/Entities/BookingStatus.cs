
namespace AirportManagement.Domain.Entities
{
    public class BookingStatus
    {
        public int Id { get; set; }

        public string Status { get; set; } = null!;

        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
