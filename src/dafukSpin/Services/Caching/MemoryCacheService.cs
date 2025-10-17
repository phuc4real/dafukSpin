using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Text.Json;

namespace dafukSpin.Services.Caching;

/// <summary>
/// Memory cache implementation of ICacheService
/// </summary>
public sealed class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;

    public string ProviderType => "MemoryCache";

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var success = _memoryCache.TryGetValue(key, out var value);
            if (success && value is T typedValue)
            {
                _logger.LogDebug("Memory cache hit for key: {Key}", key);
                return Task.FromResult<T?>(typedValue);
            }

            _logger.LogDebug("Memory cache miss for key: {Key}", key);
            return Task.FromResult<T?>(null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from memory cache for key: {Key}", key);
            return Task.FromResult<T?>(null);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan expiration, CacheItemPriority priority = CacheItemPriority.Normal, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Size = 1,
                Priority = priority
            };

            _memoryCache.Set(key, value, options);
            _logger.LogDebug("Set value in memory cache for key: {Key} with expiration: {Expiration}", key, expiration);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in memory cache for key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Removed key from memory cache: {Key}", key);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key from memory cache: {Key}", key);
            return Task.CompletedTask;
        }
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_memoryCache is MemoryCache memoryCache)
            {
                // Reflection-based approach to clear memory cache
                var field = typeof(MemoryCache).GetField("_coherentState",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (field != null)
                {
                    var coherentState = field.GetValue(memoryCache);
                    var entriesCollection = coherentState?.GetType()
                        .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                    if (entriesCollection?.GetValue(coherentState) is IDictionary entries)
                    {
                        entries.Clear();
                        _logger.LogInformation("Memory cache cleared successfully");
                    }
                }
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing memory cache");
            return Task.CompletedTask;
        }
    }

    public Task<CacheStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var additionalInfo = new Dictionary<string, object>
        {
            ["note"] = "Memory cache statistics are limited with standard IMemoryCache interface",
            ["implementation"] = _memoryCache.GetType().Name
        };

        var stats = new CacheStats(
            ProviderType,
            DateTime.UtcNow,
            "Active",
            additionalInfo
        );

        return Task.FromResult(stats);
    }
}