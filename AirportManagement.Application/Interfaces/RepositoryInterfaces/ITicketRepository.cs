using DomainTicket = AirportManagement.Domain.Entities.Ticket;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface ITicketRepository : IGenericRepository<DomainTicket>
    {
        Task<int> GetSoldSeatsCountAsync(int flightScheduleId);

        Task<IReadOnlyList<DomainTicket>> GetByFlightScheduleAsync(int flightScheduleId);

        Task<DomainTicket?> GetByBookingIdAsync(int bookingId);
    }
}
