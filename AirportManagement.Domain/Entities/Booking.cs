
namespace AirportManagement.Domain.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; } = null!;

        public int BookingStatusId { get; set; }

        public DateTime CreatedUtc { get; set; }

        public string ConfirmationCode { get; set; } = null!;

        public int Quantity { get; set; }

        public virtual BookingStatus BookingStatus { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    }

}
