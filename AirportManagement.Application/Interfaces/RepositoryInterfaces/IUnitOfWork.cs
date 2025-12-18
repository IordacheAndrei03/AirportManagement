using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IUnitOfWork
    {
        IFlightRepository FlightRepository { get; }

        IFlightScheduleRepository FlightScheduleRepository { get; }

        ITicketRepository TicketRepository { get; }

        IBookingRepository BookingRepository { get; }
        
        IAirlineRepository AirlineRepository { get; }

        IAirportRepository AirportRepository { get; }

        IAircraftRepository AircraftRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
