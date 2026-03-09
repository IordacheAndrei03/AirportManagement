
namespace AirportManagement.Application.Dtos.BookingDtos
{
    public class BookingDetailsDto
    {
        public string ConfirmationCode { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }

        public int Quantity { get; set; }

        public int FlightScheduleId { get; set; }

        public int TicketId { get; set; }

        public string PassengerFullName { get; set; } = string.Empty;

        public string PassengerEmail { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "EUR";
    }
}
