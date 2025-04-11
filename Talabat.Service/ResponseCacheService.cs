using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StackExchange.Redis;
using Talabat.Core.IServices;

namespace Talabat.Service
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null)
                return;
            var serializerOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var responseSerializer = JsonSerializer.Serialize(response, serializerOptions);
            await _database.StringSetAsync(cacheKey, responseSerializer, timeToLive);
        }

        public async Task<string?> GetCacheResponseAsync(string cacheKey)
        {
            var response = await _database.StringGetAsync(cacheKey);
            if (response.IsNullOrEmpty)
                return null;
            return response;
        }
    }
}
