using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DomainAirline = AirportManagement.Domain.Entities.Airline;
using EfAirline = AirportManagement.Infrastructure.ScaffoldDb.Entities.Airline;

namespace AirportManagement.Infrastructure.Repositories
{
    public class AirlineRepository: GenericRepository<DomainAirline, EfAirline>, IAirlineRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public AirlineRepository(AirportManagementContext context, IMapper mapper):base(context,mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<DomainAirline?> GetByIataCodeAsync(string iataCode)
        {
            var efEntity = await _context.Airlines
                .FirstOrDefaultAsync(a => a.Iatacode == iataCode);

            return _mapper.Map<DomainAirline?>(efEntity);
        }
    }
}
