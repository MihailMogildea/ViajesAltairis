using System.Collections.Concurrent;
using ViajesAltairis.Application.Interfaces;

namespace ViajesAltairis.Admin.Api.Tests.Infrastructure;

public class FakeCacheService : ICacheService
{
    private readonly ConcurrentDictionary<string, object> _store = new();

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (_store.TryGetValue(key, out var value))
            return Task.FromResult((T?)value);
        return Task.FromResult(default(T));
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
    {
        _store[key] = value!;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        foreach (var key in _store.Keys)
            if (key.StartsWith(prefix))
                _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
