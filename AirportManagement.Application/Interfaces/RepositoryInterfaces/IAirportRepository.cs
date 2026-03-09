using DomainAirport = AirportManagement.Domain.Entities.Airport;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAirportRepository:IGenericRepository<DomainAirport>
    {
        public Task<DomainAirport?> GetByIataCodeAsync(string iataCode);
    }
}
