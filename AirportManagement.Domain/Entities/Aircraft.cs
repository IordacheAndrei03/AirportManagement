using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Domain.Entities
{
    public class Aircraft
    {
        public int Id { get; set; }

        public string TailNumber { get; set; } = null!;

        public string Model { get; set; } = null!;

        public int SeatCapacity { get; set; }

        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();

        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
