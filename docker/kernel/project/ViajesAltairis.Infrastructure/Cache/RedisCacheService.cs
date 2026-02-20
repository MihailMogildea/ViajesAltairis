using System.Text.Json;
using StackExchange.Redis;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;

    public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _database = connectionMultiplexer.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var value = await _database.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        var serialized = JsonSerializer.Serialize(value);
        await _database.StringSetAsync(key, serialized, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var server = _database.Multiplexer.GetServers().First();
        var keys = new List<RedisKey>();

        await foreach (var key in server.KeysAsync(_database.Database, $"{prefix}*"))
        {
            keys.Add(key);
            if (keys.Count >= 100)
            {
                await _database.KeyDeleteAsync(keys.ToArray());
                keys.Clear();
            }
        }

        if (keys.Count > 0)
            await _database.KeyDeleteAsync(keys.ToArray());
    }
}
