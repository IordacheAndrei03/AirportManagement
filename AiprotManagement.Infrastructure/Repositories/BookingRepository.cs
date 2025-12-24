using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Entities;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainBooking = AirportManagement.Domain.Entities.Booking;
using EfBooking = AirportManagement.Infrastructure.ScaffoldDb.Entities.Booking;
using DomainBookingStatus = AirportManagement.Domain.Entities.BookingStatus;
using EfBookingStatus = AirportManagement.Infrastructure.ScaffoldDb.Entities.BookingStatus;

namespace AirportManagement.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<DomainBooking, EfBooking>, IBookingRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public BookingRepository(AirportManagementContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<DomainBooking?> GetByConfirmationCodeAsync(string confirmationCode)
        {
            var efEntity = await _context.Bookings
              .Include(b => b.BookingStatus)
              .AsNoTracking()
              .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode);

            return _mapper.Map<DomainBooking>(efEntity);
        }

        public Task<bool> HasActiveBookingsForTicketAsync(int ticketId)
        {
            return _context.Tickets
                .AsNoTracking()
                .AnyAsync(t =>
                    t.Id == ticketId &&
                    t.Booking != null &&
                    t.Booking.BookingStatus != null &&
                    t.Booking.BookingStatus.Status == "Active");
        }

        public async Task<DomainBookingStatus?> GetByStatusAsync(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                throw new ArgumentException("Status cannot be null or empty.", nameof(status));

            var efEntity = await _context.BookingStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Status == status);

            return _mapper.Map<DomainBookingStatus>(efEntity);
        }

        public async Task<DomainBooking?> GetByIdWithStatusAsync(int id)
        {
            var efEntity = await _context.Bookings
                .Include(b => b.BookingStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);

            return _mapper.Map<DomainBooking>(efEntity);

        }

        public async Task IncrementQuantityAsync(int bookingId)
        {
            var updated = await _context.Bookings
                .Where(b => b.Id == bookingId)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.Quantity, b => b.Quantity + 1));

            if (updated == 0)
                throw new KeyNotFoundException($"Booking {bookingId} not found.");
        }
    }
}
