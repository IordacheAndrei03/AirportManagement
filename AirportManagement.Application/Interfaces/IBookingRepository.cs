using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainBooking = AirportManagement.Domain.Entities.Booking;

namespace AirportManagement.Application.Interfaces
{
    public interface IBookingRepository : IGenericRepository<DomainBooking>
    {
        Task<Booking?> GetByConfirmationCodeAsync(
            string confirmationCode,
            CancellationToken cancellationToken = default);

        Task<Booking?> GetWithTicketsByConfirmationCodeAsync(
            string confirmationCode,
            CancellationToken cancellationToken = default);
    }
}
