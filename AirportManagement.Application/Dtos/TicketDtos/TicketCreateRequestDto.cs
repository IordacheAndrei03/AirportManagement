
namespace AirportManagement.Application.Dtos.TicketDtos
{
    public class TicketCreateRequestDto
    {
        public int BookingId { get; set; }

        public int FlightScheduleId { get; set; }

        public string FareClass { get; set; } = null!;

        public decimal BasePrice { get; set; }

        public decimal Taxes { get; set; }

        public bool IsRefundable { get; set; }

        public string SeatNumber { get; set; } = null!;

        public string PassengerFullName { get; set; } = null!;

        public string PassengerEmail { get; set; } = null!;
    }
}
