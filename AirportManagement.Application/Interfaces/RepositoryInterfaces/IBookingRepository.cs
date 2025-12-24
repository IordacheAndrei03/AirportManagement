using AirportManagement.Domain.Entities;
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
