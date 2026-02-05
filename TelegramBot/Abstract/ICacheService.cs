namespace TelegramBot.Abstract;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key);

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    public Task<bool> RemoveAsync(string key);

    public Task<bool> ExistsAsync(string key);
}
