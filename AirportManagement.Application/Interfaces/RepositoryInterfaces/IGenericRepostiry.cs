using System;
using System.Collections.Generic;
using System.Linq;
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
        //Task<IEnumerable<TDomain>> FindAsync(Func<TDomain, bool> pred);
        Task AddAsync(TDomain entity);
        Task AddRangeAsync(IEnumerable<TDomain> entities);
        void Update(TDomain entity);
        void Delete(TDomain entity);
        //IQueryable<TEf> Query();
    }
}
