using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainGate = AirportManagement.Domain.Entities.Gate;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IGateRepository:IGenericRepository<DomainGate>
    {
        Task<DomainGate?> GetByAirportIdAndCodeAsync(int airportId, string gateCode);
    }
}
