using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace CampusMatch.Api.Services;

/// <summary>
/// Redis-backed distributed cache service implementation
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key) where T : class
    {
        try
        {
            var data = await _cache.GetStringAsync(key);
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }
            
            return JsonSerializer.Deserialize<T>(data, _jsonOptions);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache get failed for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
            };

            var json = JsonSerializer.Serialize(value, _jsonOptions);
            await _cache.SetStringAsync(key, json, options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache set failed for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache remove failed for key: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Note: Pattern-based removal requires Redis server commands
        // For IDistributedCache, we'll log that this is not supported
        _logger.LogDebug("RemoveByPattern called with pattern: {Pattern}. Pattern removal requires direct Redis connection.", pattern);
        await Task.CompletedTask;
    }

    public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class
    {
        var cached = await GetAsync<T>(key);
        if (cached != null)
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

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            var data = await _cache.GetStringAsync(key);
            return !string.IsNullOrEmpty(data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cache exists check failed for key: {Key}", key);
            return false;
        }
    }
}
