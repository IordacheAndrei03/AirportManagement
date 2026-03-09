
namespace AirportManagement.Application.Dtos.TicketDtos
{
    public class TicketByFlightScheduleDto
    {
        public string FareClass { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public string Currency { get; set; } = string.Empty;

        public bool IsRefundable { get; set; }

        public string SeatNumber { get; set; } = string.Empty;
    }
}
