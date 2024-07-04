using IdentityServer.Shared.Client.Repositories.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace IdentityServer.Shared.Client.Repositories
{
    public class JwtCacheRepository(IConnectionMultiplexer redis) : IJwtCacheRepository
    {
        private readonly IDatabase _redisCli= redis.GetDatabase();
        

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

            await _redisCli.StringSetAsync(key, content,expiry );
        }

        public async Task RemoveAsync(string key)
        {
            await _redisCli.KeyDeleteAsync(key);
        }
    }
}

