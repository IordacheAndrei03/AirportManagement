using AirportManagement.Application.Enums;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories
{
    public class FlightStatusRepository : IFlightStatusRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;

        public FlightStatusRepository(AirportManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> GetStatusIdByNameAsync(FlightScheduleStatus status)
        {
            var id = await _context.FlightStatuses
                .Where(s => s.Status == status.ToString())
                .Select(s => s.Id)
                .FirstOrDefaultAsync();

            return id;
        }
    }
}
