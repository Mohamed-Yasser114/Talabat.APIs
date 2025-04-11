using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.IServices
{
    public interface IResponseCacheService
    {
        public Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        public Task<string?> GetCacheResponseAsync(string cacheKey);
    }
}
