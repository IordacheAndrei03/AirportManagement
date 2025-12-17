using AirportManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DomainFlight = AirportManagement.Domain.Entities.Flight;


namespace AirportManagement.Application.Interfaces
{
    public interface IFlightRepository : IGenericRepository<DomainFlight>
    {
        Task<Flight?> GetByIdWithDetailsAsync(int id);
        Task<bool> ExistsDuplicateRouteAsync(
           int airlineId,
           string flightNumber,
           int originAirportId,
           int destinationAirportId,
           int? excludeFlightId = null);
        Task<Flight?> GetByBusinessKeyAsync(
            int airlineId,
            string flightNumber,
            int originAirportId,
            int destinationAirportId);
    }
}
