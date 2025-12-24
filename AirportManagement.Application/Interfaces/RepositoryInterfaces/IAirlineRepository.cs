using DomainAirline = AirportManagement.Domain.Entities.Airline;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAirlineRepository:IGenericRepository<DomainAirline>
    {
        public Task<DomainAirline?> GetByIataCodeAsync(string iataCode);
    }
}
