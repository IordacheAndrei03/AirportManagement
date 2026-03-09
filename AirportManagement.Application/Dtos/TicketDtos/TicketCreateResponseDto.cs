
namespace AirportManagement.Application.Dtos.TicketDtos
{
    public class TicketCreateResponseDto
    {
        public string FareClass { get; set; } = null!;

        public decimal TotalPrice { get; set; }

        public bool IsRefundable { get; set; }

        public string SeatNumber { get; set; } = null!;

        public string PassengerFullName { get; set; } = null!;

        public string PassengerEmail { get; set; } = null!;
    }
}
