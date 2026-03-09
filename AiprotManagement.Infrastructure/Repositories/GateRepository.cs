using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DomainGate = AirportManagement.Domain.Entities.Gate;
using EfGate = AirportManagement.Infrastructure.ScaffoldDb.Entities.Gate;

namespace AirportManagement.Infrastructure.Repositories
{
    public class GateRepository : GenericRepository<DomainGate, EfGate>, IGateRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public GateRepository(AirportManagementContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
        }

        public async Task<DomainGate?> GetByAirportIdAndCodeAsync(int airportId, string gateCode)
        {
            var efEntity = await _context.Gates
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.AirportId == airportId && g.Code == gateCode);

            return _mapper.Map<DomainGate?>(efEntity);
        }
    }
}
