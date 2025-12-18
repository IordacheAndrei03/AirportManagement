using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAircraft = AirportManagement.Domain.Entities.Aircraft;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAircraftRepository:IGenericRepository<DomainAircraft>
    {
        public Task<bool> IsTailNoExistsAsync(string tailNo);

        public Task<DomainAircraft?> GetByTailNoAsync(string tailNo);
    }
}
