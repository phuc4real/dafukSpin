using dafukSpin.Services.Caching;
using StackExchange.Redis;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring caching services
/// </summary>
public static class CachingExtensions
{
    /// <summary>
    /// Configures caching for the dafukSpin application with Redis support and Memory cache fallback
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinCaching(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // Always add memory cache as fallback
        services.AddMemoryCache(options =>
        {
            options.SizeLimit = 1000; // Maximum number of cached items
            options.CompactionPercentage = 0.25; // Remove 25% of items when size limit is reached
        });

        // Check for Redis configuration
        var redisConnectionString = configuration?.GetConnectionString("Redis");
        var useRedis = !string.IsNullOrEmpty(redisConnectionString);

        if (useRedis)
        {
            try
            {
                // Add Redis distributed cache
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = redisConnectionString;
                    options.InstanceName = "dafukSpin";
                });

                // Add Redis connection multiplexer for advanced operations
                services.AddSingleton<IConnectionMultiplexer>(provider =>
                {
                    return ConnectionMultiplexer.Connect(redisConnectionString!);
                });

                // Register cache services
                services.AddSingleton<MemoryCacheService>();
                services.AddSingleton<RedisCacheService>();

                // Use hybrid cache as the main cache service
                services.AddSingleton<ICacheService>(provider =>
                {
                    var redisCache = provider.GetRequiredService<RedisCacheService>();
                    var memoryCache = provider.GetRequiredService<MemoryCacheService>();
                    var logger = provider.GetRequiredService<ILogger<HybridCacheService>>();

                    return new HybridCacheService(redisCache, memoryCache, logger);
                });

                // Log that Redis is configured
                services.AddSingleton<IHostedService, CacheStartupService>();
            }
            catch (Exception)
            {
                // If Redis configuration fails, fall back to memory cache only
                useRedis = false;
            }
        }

        if (!useRedis)
        {
            // Use only memory cache
            services.AddSingleton<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}

/// <summary>
/// Hosted service to log cache configuration on startup
/// </summary>
internal sealed class CacheStartupService : IHostedService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CacheStartupService> _logger;

    public CacheStartupService(ICacheService cacheService, ILogger<CacheStartupService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            var stats = await _cacheService.GetStatsAsync(cancellationToken);
            _logger.LogInformation("Cache service initialized: {ProviderType} - Status: {Status}",
                stats.ProviderType, stats.Status);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get cache statistics on startup");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}