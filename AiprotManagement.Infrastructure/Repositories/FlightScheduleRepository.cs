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

        public async Task<DomainFlightSchedule?> GetByIdWithDetailsAsync(int id)
        {
            var efEntity = await _context.FlightSchedules
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.Airline)
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.OriginAirportNavigation)
                .Include(fs => fs.Flight)
                    .ThenInclude(f => f.DestinationAirportNavigation)
                .Include(fs => fs.Gate)
                .Include(fs => fs.AssignedAircraft)
                .Include(fs => fs.FlightStatus)
                .AsNoTracking()
                .FirstOrDefaultAsync(fs => fs.Id == id);
            
            return _mapper.Map<DomainFlightSchedule?>(efEntity);
        }

        //public async Task<IReadOnlyList<DomainFlightSchedule>> SearchUpcomingByRouteAndDateAsync(
        //    string originIata,
        //    string destinationIata,
        //    DateTime departureDateUtc,
        //    int page,
        //    int pageSize)
        //{
        //    if (page <= 0) page = 1;
        //    if (pageSize <= 0) pageSize = 20;

        //    var start = departureDateUtc.Date;
        //    var end = start.AddDays(1);

        //    var query = await _context.FlightSchedules
        //        .Include(fs => fs.Flight)
        //            .ThenInclude(f => f.Airline)
        //        .Include(fs => fs.Flight)
        //            .ThenInclude(f => f.OriginAirportNavigation)
        //        .Include(fs => fs.Flight)
        //            .ThenInclude(f => f.DestinationAirportNavigation)
        //        .Where(fs =>
        //            fs.ScheduledDepartureUtc >= start &&
        //            fs.ScheduledDepartureUtc < end &&
        //            fs.Flight.OriginAirportNavigation.IATACode == originIata &&
        //            fs.Flight.DestinationAirportNavigation.IATACode == destinationIata)
        //        .OrderBy(fs => fs.ScheduledDepartureUtc)
        //        .AsNoTracking();

        //    return await query
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //}

        public async Task<bool> HasGateOverlapAsync(
            int gateId,
            DateTime fromUtc,
            DateTime toUtc,
            int? ignoreScheduleId = null)
        {
            var query = _context.FlightSchedules
                .Where(fs => fs.GateId == gateId);

            if (ignoreScheduleId.HasValue)
            {
                query = query.Where(fs => fs.Id != ignoreScheduleId.Value);
            }

            return await query.AnyAsync(fs =>
                    fs.ScheduledDepartureUtc < toUtc &&
                    fs.ScheduleArrivalUtc > fromUtc);
        }

    }
}
