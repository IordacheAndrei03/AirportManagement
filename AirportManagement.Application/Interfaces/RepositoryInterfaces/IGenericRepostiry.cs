using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IGenericRepository<TDomain> 
        where TDomain : class
    {
        Task<TDomain> GetByIdAsync(int id);

        Task<IEnumerable<TDomain>> GetAllAsync();

        Task<IEnumerable<TDomain>> GetPagedAsync(int page, int pageSize);

        Task AddAsync(TDomain entity);

        Task AddRangeAsync(IEnumerable<TDomain> entities);

        void Update(TDomain entity);

        void Delete(TDomain entity);

        Task<bool> DeleteByIdAsync(int id);


    }
}
