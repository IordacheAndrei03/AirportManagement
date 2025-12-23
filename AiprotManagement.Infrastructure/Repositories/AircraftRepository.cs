using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DomainAircraft = AirportManagement.Domain.Entities.Aircraft;
using EfAircraft = AirportManagement.Infrastructure.ScaffoldDb.Entities.Aircraft;


namespace AirportManagement.Infrastructure.Repositories
{
    public class AircraftRepository : GenericRepository<DomainAircraft, EfAircraft>, IAircraftRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public AircraftRepository(AirportManagementContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> IsTailNoExistsAsync(string tailNo)
        {
            return await _context.Aircraft
                .AnyAsync(a => a.TailNumber == tailNo);
        }

        public async Task<DomainAircraft?> GetByTailNoAsync(string tailNo)
        {
            var efEntity = await _context.Aircraft
                .FirstOrDefaultAsync(a => a.TailNumber == tailNo);

            return _mapper.Map<DomainAircraft?>(efEntity);
        }
    }
}
