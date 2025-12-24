using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
