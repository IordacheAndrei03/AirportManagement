using AirportManagement.Application.Interfaces.RepositoryInterfaces;
using AirprotManagement.Infrastructure.ScaffoldDb.Context;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

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

        public Task AddRangeAsync(IEnumerable<TDomain> entities)
        {
            throw new NotImplementedException();
        }

        public void Delete(TDomain entity)
        {
            throw new NotImplementedException();
        }

        //public Task<IEnumerable<TDomain>> FindAsync(Func<TDomain, bool> predicate)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<IEnumerable<TDomain>> GetAllAsync()
        {
            throw new NotImplementedException();
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

        public async Task<IEnumerable<TDomain>> GetPagedAsync(int page, int pageSize)
        {
            var efEntity = await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<TDomain>>(efEntity);
        }

        public IQueryable<TDomain> Query()
        {
            throw new NotImplementedException();
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
