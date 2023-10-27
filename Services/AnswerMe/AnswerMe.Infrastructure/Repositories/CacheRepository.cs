using System.Text;
using System.Text.Json;
using AnswerMe.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace AnswerMe.Infrastructure.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private readonly IDistributedCache _distributedCache;

        public CacheRepository(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var responseJson = await _distributedCache.GetAsync(key);

            if (responseJson != null)
            {
                return JsonSerializer.Deserialize<T>(responseJson);
            }
            return default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(expiry);

            var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

            await _distributedCache.SetAsync(key, content, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}

