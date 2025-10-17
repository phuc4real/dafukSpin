using dafukSpin.Endpoints;
using dafukSpin.Services.Caching;

namespace dafukSpin.Endpoints;

/// <summary>
/// Endpoints for cache management operations
/// </summary>
public sealed class CacheManagementEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cache")
            .WithTags("Cache Management");

        group.MapPost("/clear", ClearCacheAsync)
            .WithName("ClearCache")
            .WithSummary("Clear all cache entries")
            .WithDescription("Clears all cached data from the active cache provider")
            .Produces(200)
            .Produces(500);

        group.MapGet("/stats", GetCacheStatsAsync)
            .WithName("GetCacheStats")
            .WithSummary("Get cache statistics")
            .WithDescription("Returns cache statistics and information from the active cache provider")
            .Produces<object>(200);
    }

    private static async Task<IResult> ClearCacheAsync(ICacheService cache)
    {
        try
        {
            await cache.ClearAsync(CancellationToken.None);
            return Results.Ok(new { message = "Cache cleared successfully", timestamp = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error clearing cache: {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetCacheStatsAsync(ICacheService cache)
    {
        try
        {
            var stats = await cache.GetStatsAsync(CancellationToken.None);
            return Results.Ok(stats);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error getting cache stats: {ex.Message}", statusCode: 500);
        }
    }
}