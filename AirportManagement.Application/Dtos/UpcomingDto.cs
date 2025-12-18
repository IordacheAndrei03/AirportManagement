using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Dtos
{
    public class UpcomingDto
    {
        public DateTime Date { get; set; }
        public int Flights { get; set; }
    }
}
