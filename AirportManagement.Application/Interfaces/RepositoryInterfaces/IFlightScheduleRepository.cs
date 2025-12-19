using AirportManagement.Application.Dtos;
using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainFlightSchedule = AirportManagement.Domain.Entities.FlightSchedule;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IFlightScheduleRepository : IGenericRepository<DomainFlightSchedule>
    {
        Task<DomainFlightSchedule?> GetByIdWithDetailsAsync(int id);

        Task<IReadOnlyList<DomainFlightSchedule>> SearchUpcomingByRouteAndDateAsync(
            string originIata,
            string destinationIata,
            DateTime departureDateUtc,
            int page,
            int pageSize);

        Task<bool> HasGateOverlapAsync(
            int gateId,
            DateTime fromUtc,
            DateTime toUtc,
            int? ignoreScheduleId = null);

        Task<bool> AnyByFlightIdAsync(int flightId);

        Task<IReadOnlyList<UpcomingStatsRow>> GetUpcomingStatsAsync(int days);

        Task<DomainFlightSchedule?> FindByFlightAndDepartureAsync(
    int flightId,
    DateTime departureUtc,
    CancellationToken ct = default);
    }
}
