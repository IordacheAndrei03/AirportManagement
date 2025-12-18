using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAirport = AirportManagement.Domain.Entities.Airport;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAirportRepository:IGenericRepository<DomainAirport>
    {
        public Task<bool> IsIataCodeExistsAsync(string iataCode);

        public Task<DomainAirport?> GetByIataCodeAsync(string iataCode);
    }
}
