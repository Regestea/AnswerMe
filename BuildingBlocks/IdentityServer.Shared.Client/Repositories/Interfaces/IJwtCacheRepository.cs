namespace IdentityServer.Shared.Client.Repositories.Interfaces;

/// <summary>
/// Represents a repository for caching and retrieving data using JWT as the key.
/// </summary>
public interface IJwtCacheRepository
{
    /// <summary>
    /// Retrieves the value associated with the specified key asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the value to be retrieved.</typeparam>
    /// <param name="key">The key of the value to be retrieved.</param>
    /// <returns>
    /// A Task representing the asynchronous operation with the retrieved value,
    /// or null if the key does not exist in the data source.
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// Sets the value of an item in the cache asynchronously with a specified expiry.
    /// @param key The key of the item to set.
    /// @param value The value of the item to set.
    /// @param expiry The expiration duration of the item.
    /// @returns A Task representing the asynchronous operation.
    /// /
    Task SetAsync<T>(string key, T value, TimeSpan expiry);

    /// <summary>
    /// Asynchronously removes an item from the cache using the specified key.
    /// </summary>
    /// <param name="key">The key of the item to remove from the cache.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}