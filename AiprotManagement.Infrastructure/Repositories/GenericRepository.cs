using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirportManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AirportManagement.Infrastructure.Repositories
{
    public class GenericRepository<TDomain, TEf> : IGenericRepository<TDomain>
        where TDomain : class
        where TEf : class
    {
        private readonly AirportManagementContext _context;
        private readonly DbSet<TEf> _dbSet;
        protected IMapper _mapper;

        public GenericRepository(AirportManagementContext context, IMapper mapper)
        {
            _context = context;
            _dbSet = _context.Set<TEf>();
            _mapper = mapper;
        }

        public async Task AddAsync(TDomain entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var efEntity = _mapper.Map<TEf>(entity);

            await _dbSet.AddAsync(efEntity);
        }

        public virtual async Task<TDomain> GetByIdAsync(int id)
        {
            var efEntity = await _dbSet.FindAsync(id);

            if (efEntity == null)
            {
                return default;
            }

            _context.Entry(efEntity).State = EntityState.Detached;

            return _mapper.Map<TDomain>(efEntity);
        }

        public async void Update(TDomain entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            _dbSet.Update(_mapper.Map<TEf>(entity));
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            var efEntity = await _dbSet.FindAsync(id);

            if (efEntity == null)
            {
                return false;
            }

            _dbSet.Remove(efEntity);

            return true;
        }
    }
}
