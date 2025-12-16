using System;
using System.Collections.Generic;

namespace AirprotManagement.Infrastructure.ScaffoldDb.Entities;

public partial class FlightSchedule
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
