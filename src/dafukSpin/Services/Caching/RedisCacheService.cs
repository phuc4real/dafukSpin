using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;
using StackExchange.Redis;

namespace dafukSpin.Services.Caching;

/// <summary>
/// Redis cache implementation of ICacheService
/// </summary>
public sealed class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _distributedCache;
    private readonly IConnectionMultiplexer? _connectionMultiplexer;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public string ProviderType => "Redis";

    public RedisCacheService(
        IDistributedCache distributedCache,
        ILogger<RedisCacheService> logger,
        IConnectionMultiplexer? connectionMultiplexer = null)
    {
        _distributedCache = distributedCache;
        _connectionMultiplexer = connectionMultiplexer;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (!string.IsNullOrEmpty(json))
            {
                var value = JsonSerializer.Deserialize<T>(json, _jsonOptions);
                if (value != null)
                {
                    _logger.LogDebug("Redis cache hit for key: {Key}", key);
                    return value;
                }
            }

            _logger.LogDebug("Redis cache miss for key: {Key}", key);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting value from Redis cache for key: {Key}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, CacheItemPriority priority = CacheItemPriority.Normal, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            var json = JsonSerializer.Serialize(value, _jsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _distributedCache.SetStringAsync(key, json, options, cancellationToken);
            _logger.LogDebug("Set value in Redis cache for key: {Key} with expiration: {Expiration}", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting value in Redis cache for key: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            _logger.LogDebug("Removed key from Redis cache: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing key from Redis cache: {Key}", key);
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_connectionMultiplexer != null && _connectionMultiplexer.IsConnected)
            {
                var database = _connectionMultiplexer.GetDatabase();
                var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

                // Warning: FLUSHDB clears the entire database. Use with caution in production.
                await server.FlushDatabaseAsync(database.Database);
                _logger.LogWarning("Redis database cleared. All cache entries removed.");
            }
            else
            {
                _logger.LogWarning("Cannot clear Redis cache: connection not available");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing Redis cache");
        }
    }

    public async Task<CacheStats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        var additionalInfo = new Dictionary<string, object>();
        var status = "Unknown";

        try
        {
            if (_connectionMultiplexer != null)
            {
                status = _connectionMultiplexer.IsConnected ? "Connected" : "Disconnected";
                additionalInfo["connectionStatus"] = status;
                additionalInfo["endpoints"] = _connectionMultiplexer.GetEndPoints().Select(ep => ep.ToString()).ToArray();

                if (_connectionMultiplexer.IsConnected)
                {
                    var database = _connectionMultiplexer.GetDatabase();
                    var server = _connectionMultiplexer.GetServer(_connectionMultiplexer.GetEndPoints().First());

                    var info = await server.InfoAsync("memory");
                    var memoryInfo = info.Where(g => g.Key == "memory").FirstOrDefault();
                    if (memoryInfo != null)
                    {
                        foreach (var item in memoryInfo)
                        {
                            if (item.Key.StartsWith("used_memory"))
                            {
                                additionalInfo[item.Key] = item.Value;
                            }
                        }
                    }
                }
            }
            else
            {
                status = "No Connection Multiplexer";
                additionalInfo["note"] = "Using IDistributedCache without direct Redis connection";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting Redis cache statistics");
            status = "Error";
            additionalInfo["error"] = ex.Message;
        }

        return new CacheStats(
            ProviderType,
            DateTime.UtcNow,
            status,
            additionalInfo
        );
    }
}