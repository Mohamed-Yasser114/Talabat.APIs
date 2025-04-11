using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.IRepositories;

namespace Talabat.Core
{
    public interface IUnitOfWork: IAsyncDisposable
    {
        public IGenericRepository<T> repository<T>() where T : BaseEntity;
        public Task<int> CompleteAsync();
    }
}
