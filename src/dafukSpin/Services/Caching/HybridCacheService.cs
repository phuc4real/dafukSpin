using Microsoft.Extensions.Caching.Memory;

namespace dafukSpin.Services.Caching;

/// <summary>
/// Hybrid cache service that uses Redis as primary and Memory cache as fallback
/// </summary>
public sealed class HybridCacheService : ICacheService
{
    private readonly ICacheService _primaryCache;
    private readonly ICacheService _fallbackCache;
    private readonly ILogger<HybridCacheService> _logger;

    public string ProviderType => $"Hybrid ({_primaryCache.ProviderType} -> {_fallbackCache.ProviderType})";

    public HybridCacheService(
        ICacheService primaryCache,
        ICacheService fallbackCache,
        ILogger<HybridCacheService> logger)
    {
        _primaryCache = primaryCache;
        _fallbackCache = fallbackCache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // Try primary cache first
            var result = await _primaryCache.GetAsync<T>(key, cancellationToken);
            if (result != null)
            {
                return result;
            }

            // Fallback to secondary cache
            result = await _fallbackCache.GetAsync<T>(key, cancellationToken);
            if (result != null)
            {
                _logger.LogDebug("Cache hit from fallback cache for key: {Key}", key);
                // Optionally, write back to primary cache
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _primaryCache.SetAsync(key, result, TimeSpan.FromMinutes(5), CacheItemPriority.Normal, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to write back to primary cache for key: {Key}", key);
                    }
                }, cancellationToken);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in hybrid cache get for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CacheItemPriority priority = CacheItemPriority.Normal, CancellationToken cancellationToken = default) where T : class
    {
        // Write to both caches concurrently
        var primaryTask = _primaryCache.SetAsync(key, value, expiration, priority, cancellationToken);
        var fallbackTask = _fallbackCache.SetAsync(key, value, expiration, priority, cancellationToken);

        try
        {
            await Task.WhenAll(primaryTask, fallbackTask);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Some cache writes failed for key: {Key}", key);
            // Continue execution even if some cache writes fail
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        // Remove from both caches concurrently
        var primaryTask = _primaryCache.RemoveAsync(key, cancellationToken);
        var fallbackTask = _fallbackCache.RemoveAsync(key, cancellationToken);

        try
        {
            await Task.WhenAll(primaryTask, fallbackTask);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Some cache removals failed for key: {Key}", key);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        // Clear both caches concurrently
        var primaryTask = _primaryCache.ClearAsync(cancellationToken);
        var fallbackTask = _fallbackCache.ClearAsync(cancellationToken);

        try
        {
            await Task.WhenAll(primaryTask, fallbackTask);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Some cache clears failed");
        }
    }

    public async Task<CacheStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var primaryStats = await _primaryCache.GetStatsAsync(cancellationToken);
            var fallbackStats = await _fallbackCache.GetStatsAsync(cancellationToken);

            var combinedInfo = new Dictionary<string, object>
            {
                ["primaryCache"] = new
                {
                    type = primaryStats.ProviderType,
                    status = primaryStats.Status,
                    info = primaryStats.AdditionalInfo
                },
                ["fallbackCache"] = new
                {
                    type = fallbackStats.ProviderType,
                    status = fallbackStats.Status,
                    info = fallbackStats.AdditionalInfo
                }
            };

            return new CacheStats(
                ProviderType,
                DateTime.UtcNow,
                "Active",
                combinedInfo
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting hybrid cache statistics");
            return new CacheStats(
                ProviderType,
                DateTime.UtcNow,
                "Error",
                new Dictionary<string, object> { ["error"] = ex.Message }
            );
        }
    }
}