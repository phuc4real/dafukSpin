using Microsoft.Extensions.Caching.Memory;

namespace dafukSpin.Services.Caching;

/// <summary>
/// Interface for cache operations supporting multiple cache providers
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from cache
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Sets a value in cache with expiration
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CacheItemPriority priority = CacheItemPriority.Normal, CancellationToken cancellationToken = default) where T : class;

    /// <summary>
    /// Removes a value from cache
    /// </summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears all cache entries
    /// </summary>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cache statistics
    /// </summary>
    Task<CacheStats> GetStatsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the cache provider type
    /// </summary>
    string ProviderType { get; }
}

/// <summary>
/// Cache statistics information
/// </summary>
public sealed record CacheStats(
    string ProviderType,
    DateTime Timestamp,
    string Status,
    Dictionary<string, object> AdditionalInfo
);