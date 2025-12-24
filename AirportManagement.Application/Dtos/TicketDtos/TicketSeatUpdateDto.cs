using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos.TicketDtos
{
    public class TicketSeatUpdateDto
    {
        public int TicketId { get; set; }
        public string SeatNumber { get; set; } = string.Empty;
    }
}
