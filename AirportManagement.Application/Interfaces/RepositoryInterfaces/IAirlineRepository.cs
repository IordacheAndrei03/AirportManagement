using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainAirline = AirportManagement.Domain.Entities.Airline;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IAirlineRepository:IGenericRepository<DomainAirline>
    {
        public Task<bool> IsIataCodeExistsAsync(string iataCode);

        public Task<DomainAirline?> GetByIataCodeAsync(string iataCode);
    }
}
