
namespace TelegramBot.Abstract;

public class InMemoryCacheService : ICacheService
{
    private static Dictionary<string, string> Cache = new();

    public async Task<T?> GetAsync<T>(string key)
    {
        return Cache.Try
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

    public Task<bool> RemoveAsync(string key);

    public Task<bool> ExistsAsync(string key);
}
