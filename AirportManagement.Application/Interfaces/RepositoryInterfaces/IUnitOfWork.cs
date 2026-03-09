
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

        IFlightStatusRepository FlightStatusRepository { get; }

        IGateRepository GateRepository { get; }

        Task<int> SaveChangesAsync();
    }
}
