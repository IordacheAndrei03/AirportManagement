using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.ScaffoldDb.Entities;

public partial class Airline
{
    public int Id { get; set; }

    public string Iatacode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public virtual ICollection<Flight> Flights { get; set; } = new List<Flight>();
}
