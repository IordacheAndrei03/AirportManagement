
namespace AirportManagement.Domain.Entities
{
    public partial class Ticket
    {
        public int Id { get; set; }

        public int BookingId { get; set; }

        public int FlightScheduleId { get; set; }

        public string FareClass { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public decimal Taxes { get; set; }

        public decimal TotalPrice { get; set; }

        public string Currency { get; set; } = null!;

        public bool IsRefundable { get; set; }

        public string SeatNumber { get; set; } = null!;

        public string PassangerFullName { get; set; } = null!;

        public string PassangerEmail { get; set; } = null!;

        public virtual Booking Booking { get; set; } = null!;

        public virtual FlightSchedule FlightSchedule { get; set; } = null!;
    }
}

