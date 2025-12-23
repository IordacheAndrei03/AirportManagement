using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos.BookingDtos
{
    public class BookingCreateRequestDto
    {
        [Required]
        public int FlightScheduleId { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required, StringLength(100)]
        public string PassengerFullName { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string PassengerEmail { get; set; } = string.Empty;

        [Required, Range(1, 100)]
        public int Quantity { get; set; }
    }
}
