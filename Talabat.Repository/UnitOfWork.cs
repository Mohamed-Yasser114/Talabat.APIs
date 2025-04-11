using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;
using Talabat.Repository.Data;

namespace Talabat.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext _dbContext;

        private Hashtable repositories;

        public UnitOfWork(StoreContext dbContext)
        {
            _dbContext = dbContext;
            repositories = new Hashtable();
        }

        public async Task<int> CompleteAsync()
            => await _dbContext.SaveChangesAsync();

        public IGenericRepository<T> repository<T>() where T : BaseEntity
        {
            var key = typeof(T).Name;
            if (!repositories.ContainsKey(key))
            {
                var repository = new GenericRepository<T>(_dbContext);
                repositories.Add(key, repository);
            }
            return repositories[key] as IGenericRepository<T>;
        }

        public async ValueTask DisposeAsync()
            => await _dbContext.DisposeAsync();
    }
}
