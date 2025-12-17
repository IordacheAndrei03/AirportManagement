using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AirportManagement.Domain.Entities
{
    public class FlightSchedule
    {
        public int Id { get; set; }

        public int FlightId { get; set; }

        public DateTime ScheduledDepartureUtc { get; set; }

        public DateTime ScheduleArrivalUtc { get; set; }

        public int GateId { get; set; }

        public int AssignedAircraftId { get; set; }

        public int FlightStatusId { get; set; }

        public virtual Aircraft AssignedAircraft { get; set; } = null!;

        public virtual Flight Flight { get; set; } = null!;

        public virtual FlightStatus FlightStatus { get; set; } = null!;

        public virtual Gate Gate { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
