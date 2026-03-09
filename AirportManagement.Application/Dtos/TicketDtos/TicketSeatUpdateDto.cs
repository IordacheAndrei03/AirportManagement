
namespace AirportManagement.Application.Dtos.TicketDtos
{
    public class TicketSeatUpdateDto
    {
        public int TicketId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;
    }
}
