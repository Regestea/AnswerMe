namespace IdentityServer.Shared.Client.Repositories.Interfaces;

public interface IJwtCacheRepository
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiry);
    Task RemoveAsync(string key);
}