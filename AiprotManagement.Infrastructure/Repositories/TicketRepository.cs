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

using DomainTicket = AirportManagement.Domain.Entities.Ticket;
using EfTicket = AirportManagement.Infrastructure.ScaffoldDb.Entities.Ticket;

namespace AirportManagement.Infrastructure.Repositories
{
    public class TicketRepository : GenericRepository<DomainTicket, EfTicket>, ITicketRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public TicketRepository(AirportManagementContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<int> GetSoldSeatsCountAsync(int flightScheduleId)
        {
            return await _context.Tickets
                .Where(t => t.FlightScheduleId == flightScheduleId)
                .CountAsync();
        }

        public async Task<IReadOnlyList<DomainTicket>> GetByFlightScheduleAsync(int flightScheduleId)
        {
            var efEntity = await _context.Tickets
                .Where(t => t.FlightScheduleId == flightScheduleId)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IReadOnlyList<DomainTicket>>(efEntity);
        }
    }
}
