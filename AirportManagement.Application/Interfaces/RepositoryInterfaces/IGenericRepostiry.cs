
namespace AirportManagement.Application.Interfaces.RepositoryInterfaces
{
    public interface IGenericRepository<TDomain> where TDomain : class
    {
        Task<TDomain> GetByIdAsync(int id);

        Task AddAsync(TDomain entity);

        void Update(TDomain entity);

        Task<bool> DeleteByIdAsync(int id);
    }
}
