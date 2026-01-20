using System.Text.Json;

namespace CampusMatch.Api.Services;

/// <summary>
/// Cache service interface for distributed caching
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get a cached item by key
    /// </summary>
    Task<T?> GetAsync<T>(string key) where T : class;
    
    /// <summary>
    /// Set a cached item with optional expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
    
    /// <summary>
    /// Remove a cached item
    /// </summary>
    Task RemoveAsync(string key);
    
    /// <summary>
    /// Remove all cached items matching a pattern
    /// </summary>
    Task RemoveByPatternAsync(string pattern);
    
    /// <summary>
    /// Get or set a cached item - if not in cache, executes factory and caches result
    /// </summary>
    Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null) where T : class;
    
    /// <summary>
    /// Check if a key exists in cache
    /// </summary>
    Task<bool> ExistsAsync(string key);
}
