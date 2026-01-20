using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CampusMatch.Api.Services;

/// <summary>
/// In-memory cache service for development/fallback when Redis is unavailable
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        var result = _cache.TryGetValue(key, out T? value) ? value : null;
        return Task.FromResult(result);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
        };
        
        _cache.Set(key, value, options);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Memory cache doesn't support pattern-based removal natively
        _logger.LogDebug("RemoveByPattern called with pattern: {Pattern}. Pattern removal not supported in memory cache.", pattern);
        return Task.CompletedTask;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        if (_cache.TryGetValue(key, out T? cached))
        {
            _logger.LogDebug("Cache hit for key: {Key}", key);
            return cached;
        }

        _logger.LogDebug("Cache miss for key: {Key}, executing factory", key);
        var value = await factory();
        
        if (value != null)
        {
            await SetAsync(key, value, expiration);
        }
        
        return value;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.TryGetValue(key, out _));
    }
}
