using AirportManagement.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IFlightStatusRepository
    {
        Task<int> GetStatusIdByNameAsync(FlightScheduleStatus status, CancellationToken cancellationToken = default);
    }
}
