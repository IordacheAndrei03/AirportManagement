
namespace AirportManagement.Application.Dtos.BookingDtos
{
    public class BookingCreateResponseDto
    {
        public string ConfirmationCode { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public int Quantity { get; set; } = 0;
    }
}
