using AirportManagement.Application.Enums;
using AirportManagement.Application.Exceptions;
using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Infrastructure.Repositories
{
    internal class FlightStatusRepository : IFlightStatusRepository
    {
        private readonly AirportManagementContext _context;
        private readonly IMapper _mapper;
        public FlightStatusRepository(AirportManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<int> GetStatusIdByNameAsync(FlightScheduleStatus status, CancellationToken cancellationToken = default)
        {
            var id = await _context.FlightStatuses
                .Where(s => s.Status == status.ToString())
                .Select(s => s.Id)
                .FirstOrDefaultAsync(cancellationToken);

            return id;
        }
    }
}
