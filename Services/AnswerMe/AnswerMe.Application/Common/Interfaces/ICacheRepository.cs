namespace AnswerMe.Application.Common.Interfaces;

public interface ICacheRepository
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan expiry);
    Task RemoveAsync(string key);
}