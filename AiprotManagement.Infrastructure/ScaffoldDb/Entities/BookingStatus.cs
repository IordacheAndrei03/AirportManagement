using System;
using System.Collections.Generic;

namespace AirprotManagement.Infrastructure.ScaffoldDb.Entities;

public partial class BookingStatus
{
    public int Id { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
