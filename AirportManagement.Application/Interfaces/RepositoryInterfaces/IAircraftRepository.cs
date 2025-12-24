using DomainAircraft = AirportManagement.Domain.Entities.Aircraft;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAircraftRepository:IGenericRepository<DomainAircraft>
    {
        public Task<DomainAircraft?> GetByTailNoAsync(string tailNo);
    }
}
