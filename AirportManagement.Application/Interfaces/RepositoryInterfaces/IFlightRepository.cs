using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainFlight = AirportManagement.Domain.Entities.Flight;


namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IFlightRepository : IGenericRepository<DomainFlight>
    {
        Task<DomainFlight?> GetByIdWithDetailsAsync(int id);

        Task<bool> ExistsDuplicateRouteAsync(
           int airlineId,
           string flightNumber,
           int originAirportId,
           int destinationAirportId,
           int? excludeFlightId = null);

        Task<DomainFlight?> GetByBusinessKeyAsync(
            int airlineId,
            string flightNumber,
            int originAirportId,
            int destinationAirportId);

        Task<DomainFlight?> FindByAirlineNumberAndRouteAsync(
            int airlineId,
            string flightNumber,
            int originAirportId,
            int destinationAirportId,
            CancellationToken cancellationToken = default);
    }
}
