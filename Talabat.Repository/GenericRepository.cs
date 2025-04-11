using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Core.Specifications;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext _dbContext;
        public GenericRepository(StoreContext dbContex)
        {
            _dbContext = dbContex;
        }

        public async Task AddAsync(T entity)
        {
            _dbContext.AddAsync(entity);
        }

        public async Task<int> CountAsync(ISpecifications<T> spec)
        {
            return await ApplyQuerySpec(spec).CountAsync();
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplyQuerySpec(spec).ToListAsync();
        }

        public async Task<T?> GetAsync(int id)
        {
                return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
        {
            return await ApplyQuerySpec(spec).FirstOrDefaultAsync();
        }

        public void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        private IQueryable<T> ApplyQuerySpec(ISpecifications<T> spec)
        {
            return SpecificationEveluator<T>.GetQuery(_dbContext.Set<T>(), spec);
        }
    }
}
