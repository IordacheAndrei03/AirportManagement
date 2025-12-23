using AirportManagement.Application.Dtos;
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

using DomainFlightSchedule = AirportManagement.Domain.Entities.FlightSchedule;
using EfFlightSchedule = AirportManagement.Infrastructure.ScaffoldDb.Entities.FlightSchedule;

namespace AirportManagement.Infrastructure.Repositories
{
    public class FlightScheduleRepository : GenericRepository<DomainFlightSchedule, EfFlightSchedule>,IFlightScheduleRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public FlightScheduleRepository(AirportManagementContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<DomainFlightSchedule>> SearchUpcomingByRouteAndDateAsync(
            string originIata,
            string destinationIata,
            DateTime departureDateUtc,
            int page,
            int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var start = departureDateUtc.Date;
            var end = start.AddDays(1);

            var query = _context.FlightSchedules
                .Include(fs => fs.Flight).ThenInclude(f => f.Airline)
                .Include(fs => fs.Flight).ThenInclude(f => f.OriginAirportNavigation)
                .Include(fs => fs.Flight).ThenInclude(f => f.DestinationAirportNavigation)
                .Where(fs =>
                    fs.ScheduledDepartureUtc >= start &&
                    fs.ScheduledDepartureUtc < end &&
                    fs.Flight.OriginAirportNavigation.Iatacode == originIata &&   
                    fs.Flight.DestinationAirportNavigation.Iatacode == destinationIata)
                .OrderBy(fs => fs.ScheduledDepartureUtc)
                .AsNoTracking();


            var efResults = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var domainResults = _mapper.Map<IReadOnlyList<DomainFlightSchedule>>(efResults);

            return domainResults;
        }

        public async Task<bool> HasGateOverlapAsync(int gateId, DateTime scheduledDepartureUtc)
        {
            var bufferMinutes = 30;

            var windowStart = scheduledDepartureUtc.AddMinutes(-bufferMinutes);
            var windowEnd = scheduledDepartureUtc.AddMinutes(bufferMinutes);

            var hasOverlap = await _context.FlightSchedules
                .AsNoTracking()
                .Where(fs => fs.GateId == gateId)
                .AnyAsync(fs => windowStart < fs.ScheduledDepartureUtc.AddMinutes(bufferMinutes) && windowEnd > fs.ScheduledDepartureUtc.AddMinutes(-bufferMinutes));

            return hasOverlap;
        }

        public async Task<bool> AnyByFlightIdAsync(int flightId)
        {
            return await _context.FlightSchedules
                .AnyAsync(fs => fs.FlightId == flightId);
        }

        public async Task<DomainFlightSchedule?> GetByIdWithDetailsAsync(int id)
        {

            var EfEntity = await _context.FlightSchedules
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.Airline)
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.OriginAirportNavigation)
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.DestinationAirportNavigation)
                .Include(fs => fs.Gate)
                    .ThenInclude(g => g.Airport)
                .Include(fs => fs.AssignedAircraft)
                .Include(fs => fs.FlightStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(fs => fs.Id == id);

            return _mapper.Map<DomainFlightSchedule?>(EfEntity);
        }

        public async Task<IReadOnlyList<UpcomingStatsRow>> GetUpcomingStatsAsync(int days)
        {
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(days);

            var query = await _context.FlightSchedules
                .Where(fs => fs.ScheduledDepartureUtc >= start &&
                             fs.ScheduledDepartureUtc < end)
                .GroupBy(fs => fs.ScheduledDepartureUtc.Date)
                .Select(g => new UpcomingStatsRow
                {
                    Date = g.Key,
                    Flights = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return query.AsReadOnly();
        }

        public async Task<DomainFlightSchedule?> FindByFlightAndDepartureAsync(
             int flightId,
             DateTime departureUtc,
             CancellationToken ct = default)
        {
            var efEntity = await _context.FlightSchedules
                .FirstOrDefaultAsync(fs =>
                    fs.FlightId == flightId &&
                    fs.ScheduledDepartureUtc == departureUtc,
                    ct);

            return _mapper.Map<DomainFlightSchedule?>(efEntity);
        }

        public async Task<int> GetSeatCapacityAsync(int flightScheduleId)
        {
            var capacity = await _context.FlightSchedules
                .Where(fs => fs.Id == flightScheduleId)
                .Select(fs => fs.AssignedAircraft.SeatCapacity)
                .FirstOrDefaultAsync();

            return capacity; 
        }

        public async Task<int> GetActiveBookedSeatsAsync(int flightScheduleId)
        {
            var activeBookedSeats = await _context.Tickets
                .AsNoTracking()
                .Where(t =>
                    t.FlightScheduleId == flightScheduleId &&
                    t.Booking != null &&
                    t.Booking.BookingStatus != null &&
                    t.Booking.BookingStatus.Status == "Active")
                .SumAsync(t => (int?)t.Booking!.Quantity);

            return activeBookedSeats ?? 0;
        }
    }
}
