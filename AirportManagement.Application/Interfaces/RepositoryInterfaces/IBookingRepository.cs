using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainBooking = AirportManagement.Domain.Entities.Booking;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IBookingRepository : IGenericRepository<DomainBooking>
    {
        Task<DomainBooking?> GetByConfirmationCodeAsync(string confirmationCode);

        Task<bool> HasActiveBookingsForTicketAsync(int ticketId);

        Task<BookingStatus?> GetByStatusAsync(string status);

        Task<DomainBooking?> GetByIdWithStatusAsync(int id);

        Task IncrementQuantityAsync(int bookingId);
    }
}
