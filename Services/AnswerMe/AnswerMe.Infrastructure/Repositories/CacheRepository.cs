using System.Text;
using System.Text.Json;
using AnswerMe.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace AnswerMe.Infrastructure.Repositories
{
    public class CacheRepository(IConnectionMultiplexer redis) : ICacheRepository
    {
        private readonly IDatabase _redisCli = redis.GetDatabase();


        public async Task<T?> GetAsync<T>(string key)
        {
            var responseJson = await _redisCli.StringGetAsync(key);

            if (!responseJson.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<T>(responseJson.ToString());
            }

            return default(T);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiry)
        {
            var content = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value));

            await _redisCli.StringSetAsync(key, content, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _redisCli.KeyDeleteAsync(key);
        }
    }
}