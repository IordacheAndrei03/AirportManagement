using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Entities;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public BookingRepository(AirportManagementContext context,IMapper mapper)
            : base(context,mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<DomainBooking?> GetByConfirmationCodeAsync(string confirmationCode)
        {
           var EfEntity = await _context.Bookings
                .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode);
           return _mapper.Map<DomainBooking?>(EfEntity);
        }

        public async Task<DomainBooking?> GetWithTicketsByConfirmationCodeAsync(string confirmationCode)
        {
           var efEntity =  _context.Bookings
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b => b.ConfirmationCode == confirmationCode);
              return _mapper.Map<DomainBooking?>(efEntity);
        }
    }
}
