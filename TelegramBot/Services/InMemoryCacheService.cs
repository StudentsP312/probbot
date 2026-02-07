using System.Collections.Concurrent;
using System.Text.Json;
using TelegramBot.Abstract;

namespace TelegramBot.Services;

public class InMemoryCacheService : ICacheService
{
    private static readonly ConcurrentDictionary<string, CacheItem> _cache = new();

    private record CacheItem(string Value, DateTimeOffset? Expiry);

    public Task<T?> GetAsync<T>(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.Expiry.HasValue && item.Expiry.Value < DateTimeOffset.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult<T?>(default);
            }

            return Task.FromResult(JsonSerializer.Deserialize<T>(item.Value));
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var expirationTime = expiry.HasValue ? DateTimeOffset.UtcNow.Add(expiry.Value) : (DateTimeOffset?)null;
        var newItem = new CacheItem(json, expirationTime);

        _cache.AddOrUpdate(key, newItem, (_, _) => newItem);
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string key)
    {
        return Task.FromResult(_cache.TryRemove(key, out _));
    }

    public Task<bool> ExistsAsync(string key)
    {
        if (_cache.TryGetValue(key, out var item))
        {
            if (item.Expiry.HasValue && item.Expiry.Value < DateTimeOffset.UtcNow)
            {
                _cache.TryRemove(key, out _);
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}