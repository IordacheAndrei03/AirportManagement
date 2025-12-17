using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Domain.Entities
{
    public class Gate
    {
        public int Id { get; set; }

        public int AirportId { get; set; }

        public string Code { get; set; } = null!;

        public virtual Airport Airport { get; set; } = null!;

        public virtual ICollection<FlightSchedule> FlightSchedules { get; set; } = new List<FlightSchedule>();
    }
}
