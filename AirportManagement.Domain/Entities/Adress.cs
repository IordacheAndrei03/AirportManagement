using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Domain.Entities
{
    public class Adress
    {
        public int Id { get; set; }

        public string Country { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Street { get; set; } = null!;

        public virtual Airport? Airport { get; set; }
    }
}
