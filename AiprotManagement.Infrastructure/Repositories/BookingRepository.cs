using AirportManagement.Application.Interfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Entities;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using EfBooking = AirportManagement.Infrastructure.ScaffoldDb.Entities.Booking;

namespace AirportManagement.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<DomainBooking, EfBooking>, IBookingRepository
    {
        private readonly AirportManagementContext _context;

        public BookingRepository(AirportManagementContext context)
            : base(context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<DomainBooking?> GetByConfirmationCodeAsync(string confirmationCode)
        {
            return await _context.Bookings
                .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode);
        }

        public async Task<DomainBooking?> GetWithTicketsByConfirmationCodeAsync(string confirmationCode)
        {
            return await _context.Bookings
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode);
        }
    }
}
