using System.Text.Json;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TelegramBot.Abstract;

namespace TelegramBot.Services;

public class RedisService : ICacheService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(IOptions<BotConfiguration> botConfig)
    {
        var connectionString = botConfig.Value.RedisConnectionString;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("RedisConnectionString is not configured.");
        }

        _redis = ConnectionMultiplexer.Connect(connectionString);
        _db = _redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        string? value = await _db.StringGetAsync(key);
        if (string.IsNullOrEmpty(value))
            return default;

        return JsonSerializer.Deserialize<T>(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry, when: When.Always, flags: CommandFlags.None);
    }

    public async Task<bool> RemoveAsync(string key)
    {
        return await _db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await _db.KeyExistsAsync(key);
    }
}
