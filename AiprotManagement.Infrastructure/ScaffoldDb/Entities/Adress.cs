using AirprotManagement.Infrastructure.ScaffoldDb.Entities;
using System;
using System.Collections.Generic;

namespace AirportManagement.Infrastructure.ScaffoldDb.Entities;

public partial class Adress
{
    public int Id { get; set; }

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public virtual Airport? Airport { get; set; }
}
