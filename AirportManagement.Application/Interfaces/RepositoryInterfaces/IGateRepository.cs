using DomainGate = AirportManagement.Domain.Entities.Gate;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IGateRepository : IGenericRepository<DomainGate>
    {
        Task<DomainGate?> GetByAirportIdAndCodeAsync(int airportId, string gateCode);
    }
}
