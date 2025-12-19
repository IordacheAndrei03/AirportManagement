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
using DomainFlight = AirportManagement.Domain.Entities.Flight;
using EfFlight = AirportManagement.Infrastructure.ScaffoldDb.Entities.Flight;

namespace AirportManagement.Infrastructure.Repositories
{
    public class FlightRepository : GenericRepository<DomainFlight, EfFlight>, IFlightRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public FlightRepository(AirportManagementContext context, IMapper mapper) : base(context,mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;  
        }
        public async Task<DomainFlight?> GetByIdWithDetailsAsync(int id)
        {
            var efEntity = await _context.Flights
                .Include(f => f.Airline)
                .Include(f => f.OriginAirportNavigation)
                .Include(f => f.DestinationAirportNavigation)
                .Include(f => f.DefaultAircraft)
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
            return _mapper.Map<DomainFlight?>(efEntity);
        }

        public async Task<bool> ExistsDuplicateRouteAsync(
            int airlineId,
            string flightNumber,
            int originAirportId,
            int destinationAirportId,
            int? excludeFlightId = null)
        {
            var query = _context.Flights.AsQueryable();

            query = query.Where(f =>
                f.AirlineId == airlineId &&
                f.FlightNumber == flightNumber &&
                f.OriginAirport == originAirportId &&
                f.DestinationAirport == destinationAirportId);

            if (excludeFlightId.HasValue)
            {
                query = query.Where(f => f.Id != excludeFlightId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<DomainFlight?> GetByBusinessKeyAsync(
            int airlineId,
            string flightNumber,
            int originAirportId,
            int destinationAirportId)
        {
            var efEntity = await _context.Flights
                .FirstOrDefaultAsync(f =>
                    f.AirlineId == airlineId &&
                    f.FlightNumber == flightNumber &&
                    f.OriginAirport == originAirportId &&
                    f.DestinationAirport == destinationAirportId);

            return _mapper.Map<DomainFlight?>(efEntity);
        }

        public async Task<DomainFlight?> FindByAirlineNumberAndRouteAsync(
        int airlineId,
        string flightNumber,
        int originAirportId,
        int destinationAirportId,
        CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(flightNumber))
                return null;

            var normalizedFlightNumber = flightNumber.Trim().ToUpperInvariant();

            // comparăm normalized (ToUpper) pentru a evita probleme legate de case
            var efEntity = await _context.Flights
                .AsNoTracking()
                .FirstOrDefaultAsync(f =>
                    f.AirlineId == airlineId
                    && f.OriginAirport == originAirportId
                    && f.DestinationAirport == destinationAirportId
                    && EF.Functions.Like(f.FlightNumber.ToUpper(), normalizedFlightNumber),
                    cancellationToken);
            return _mapper.Map<DomainFlight?>(efEntity);
        }
    }
}

