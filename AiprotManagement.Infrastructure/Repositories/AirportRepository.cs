using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DomainAirport = AirportManagement.Domain.Entities.Airport;
using EfAirport = AirportManagement.Infrastructure.ScaffoldDb.Entities.Airport;

namespace AirportManagement.Infrastructure.Repositories
{
    public class AirportRepository:GenericRepository<DomainAirport,EfAirport>, IAirportRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public AirportRepository(AirportManagementContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DomainAirport?> GetByIataCodeAsync(string iataCode)
        {
            var efEntity = await _context.Airports
                .FirstOrDefaultAsync(a => a.Iatacode == iataCode); 

            return _mapper.Map<DomainAirport?>(efEntity);
        }
    }
}
