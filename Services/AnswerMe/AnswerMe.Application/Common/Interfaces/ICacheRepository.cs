namespace AnswerMe.Application.Common.Interfaces;

/// <summary>
/// Represents a cache repository interface.
/// </summary>
public interface ICacheRepository
{
    /// <summary>
    /// Asynchronously retrieves the value associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key used to retrieve the value.</param>
    /// <returns>
    /// A Task object that represents the asynchronous operation. The task result contains the value associated with the specified key,
    /// or null if the key does not exist.
    /// </returns>
    Task<T?> GetAsync<T>(string key);

    /// <summary>
    /// Asynchronously sets a value in the cache with the specified key and expiration time.
    /// </summary>
    /// <typeparam name="T">The type of the value to be stored in the cache.</typeparam>
    /// <param name="key">The key used to identify the value in the cache.</param>
    /// <param name="value">The value to be stored in the cache.</param>
    /// <param name="expiry">The amount of time after which the value should expire and be removed from the cache.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SetAsync<T>(string key, T value, TimeSpan expiry);

    /// <summary>
    /// Removes an item from the cache asynchronously.
    /// </summary>
    /// <param name="key">The key of the item to be removed from the cache.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    Task RemoveAsync(string key);
}