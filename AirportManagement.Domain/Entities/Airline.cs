using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Domain.Entities
{
    public class Airline
    {
        public int Id { get; set; }

        public string Iatacode { get; set; } = null!;

        public string Name { get; set; } = null!;

        public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
    }
}
