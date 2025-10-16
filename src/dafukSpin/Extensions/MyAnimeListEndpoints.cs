using dafukSpin.Models;
using dafukSpin.Services;
using Microsoft.AspNetCore.Mvc;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for mapping MyAnimeList API endpoints
/// </summary>
public static class MyAnimeListEndpoints
{
    /// <summary>
    /// Maps all MyAnimeList API endpoints to the application
    /// </summary>
    public static IEndpointRouteBuilder MapMyAnimeListEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/anime")
            .WithTags("MyAnimeList API");

        // User anime list endpoints
        group.MapGet("/users/{username}/anime", GetUserAnimeListAsync)
            .WithName("GetUserAnimeList")
            .WithSummary("Get user's anime list")
            .WithDescription("Retrieves a user's anime list with optional filtering by status and sorting")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/users/{username}/anime/completed", GetUserCompletedAnimeAsync)
            .WithName("GetUserCompletedAnime")
            .WithSummary("Get user's completed anime")
            .WithDescription("Retrieves anime that the user has completed watching")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/users/{username}/anime/watching", GetUserCurrentlyWatchingAnimeAsync)
            .WithName("GetUserCurrentlyWatchingAnime")
            .WithSummary("Get user's currently watching anime")
            .WithDescription("Retrieves anime that the user is currently watching")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/users/{username}/anime/plan-to-watch", GetUserPlanToWatchAnimeAsync)
            .WithName("GetUserPlanToWatchAnime")
            .WithSummary("Get user's plan to watch anime")
            .WithDescription("Retrieves anime that the user plans to watch")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/users/{username}/anime/on-hold", GetUserOnHoldAnimeAsync)
            .WithName("GetUserOnHoldAnime")
            .WithSummary("Get user's on-hold anime")
            .WithDescription("Retrieves anime that the user has put on hold")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/users/{username}/anime/dropped", GetUserDroppedAnimeAsync)
            .WithName("GetUserDroppedAnime")
            .WithSummary("Get user's dropped anime")
            .WithDescription("Retrieves anime that the user has dropped")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        // Anime details and search endpoints
        group.MapGet("/details/{animeId:int}", GetAnimeDetailsAsync)
            .WithName("GetAnimeDetails")
            .WithSummary("Get anime details")
            .WithDescription("Retrieves detailed information about a specific anime")
            .Produces<AnimeNode>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/search", SearchAnimeAsync)
            .WithName("SearchAnime")
            .WithSummary("Search anime")
            .WithDescription("Search for anime by title or keywords")
            .Produces<MyAnimeListResponse<AnimeSearchResult>>(200)
            .Produces(400)
            .Produces(500);

        // Rankings and seasonal endpoints
        group.MapGet("/ranking", GetAnimeRankingAsync)
            .WithName("GetAnimeRanking")
            .WithSummary("Get anime rankings")
            .WithDescription("Retrieves anime rankings by type (all, airing, upcoming, tv, ova, movie, special, bypopularity, favorite)")
            .Produces<MyAnimeListResponse<AnimeRankingEntry>>(200)
            .Produces(400)
            .Produces(500);

        group.MapGet("/seasonal/{year:int}/{season}", GetSeasonalAnimeAsync)
            .WithName("GetSeasonalAnime")
            .WithSummary("Get seasonal anime")
            .WithDescription("Retrieves anime for a specific season and year. Season values: winter, spring, summer, fall")
            .Produces<MyAnimeListResponse<AnimeSeasonEntry>>(200)
            .Produces(400)
            .Produces(500);

        group.MapGet("/suggested", GetSuggestedAnimeAsync)
            .WithName("GetSuggestedAnime")
            .WithSummary("Get suggested anime")
            .WithDescription("Retrieves anime suggestions (requires user authentication)")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(401)
            .Produces(500);

        return endpoints;
    }

    // Endpoint handler methods
    private static async Task<IResult> GetUserAnimeListAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? status = null,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserAnimeListAsync(username, status, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or has no anime list");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving anime list for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserCompletedAnimeAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserCompletedAnimeAsync(username, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or has no completed anime");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving completed anime for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserCurrentlyWatchingAnimeAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserCurrentlyWatchingAnimeAsync(username, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or is not currently watching any anime");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving currently watching anime for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserPlanToWatchAnimeAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserPlanToWatchAnimeAsync(username, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or has no plan to watch anime");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving plan to watch anime for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserOnHoldAnimeAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserOnHoldAnimeAsync(username, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or has no on-hold anime");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving on-hold anime for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetUserDroppedAnimeAsync(
        [FromRoute] string username,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetUserDroppedAnimeAsync(username, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"User '{username}' not found or has no dropped anime");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving dropped anime for user '{username}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetAnimeDetailsAsync(
        [FromRoute] int animeId,
        IMyAnimeListService service,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetAnimeDetailsAsync(animeId, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"Anime with ID '{animeId}' not found");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving anime details for ID '{animeId}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> SearchAnimeAsync(
        [FromQuery] string query,
        IMyAnimeListService service,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Results.BadRequest("Query parameter is required for anime search");
        }

        try
        {
            var result = await service.SearchAnimeAsync(query, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"No anime found for query '{query}'");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error searching anime with query '{query}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetAnimeRankingAsync(
        IMyAnimeListService service,
        [FromQuery] string rankingType = "all",
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetAnimeRankingAsync(rankingType, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"No ranking data found for type '{rankingType}'");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving anime ranking for type '{rankingType}': {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetSeasonalAnimeAsync(
        [FromRoute] int year,
        [FromRoute] string season,
        IMyAnimeListService service,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        var validSeasons = new[] { "winter", "spring", "summer", "fall" };
        if (!validSeasons.Contains(season.ToLowerInvariant()))
        {
            return Results.BadRequest($"Invalid season '{season}'. Valid seasons are: {string.Join(", ", validSeasons)}");
        }

        try
        {
            var result = await service.GetSeasonalAnimeAsync(year, season, sort, limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound($"No seasonal anime found for {season} {year}");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving seasonal anime for {season} {year}: {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetSuggestedAnimeAsync(
        IMyAnimeListService service,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetSuggestedAnimeAsync(limit, offset, cancellationToken);
            return result is not null ? Results.Ok(result) : Results.NotFound("No suggested anime found");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving suggested anime: {ex.Message}", statusCode: 500);
        }
    }
}