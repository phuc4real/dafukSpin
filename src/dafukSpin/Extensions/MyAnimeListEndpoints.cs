using Microsoft.AspNetCore.Mvc;
using dafukSpin.Models;
using dafukSpin.Services;

namespace dafukSpin.Extensions;

public static class MyAnimeListEndpoints
{
    public static void MapMyAnimeListEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/myanimelist")
            .WithTags("MyAnimeList");

        // User anime list endpoints
        MapUserAnimeListEndpoints(group);
        
        // Anime details and search endpoints
        MapAnimeEndpoints(group);
        
        // Ranking and seasonal endpoints
        MapDiscoveryEndpoints(group);
    }

    private static void MapUserAnimeListEndpoints(RouteGroupBuilder group)
    {
        // Get User's Completed Anime
        group.MapGet("/users/{username}/completed", GetUserCompletedAnime)
            .WithName("GetUserCompletedAnime")
            .WithSummary("Gets completed anime for a specific user from MyAnimeList")
            .WithDescription("Returns a list of completed anime for the specified MyAnimeList user using the official API. Optional sort parameter: list_score, list_updated_at, anime_title, anime_start_date, anime_id")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);

        // Get User's Currently Watching Anime
        group.MapGet("/users/{username}/watching", GetUserCurrentlyWatchingAnime)
            .WithName("GetUserCurrentlyWatchingAnime")
            .WithSummary("Gets currently watching anime for a specific user from MyAnimeList")
            .WithDescription("Returns a list of currently watching anime for the specified MyAnimeList user. Optional sort parameter: list_score, list_updated_at, anime_title, anime_start_date, anime_id")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);

        // Get User's Plan to Watch Anime
        group.MapGet("/users/{username}/plan-to-watch", GetUserPlanToWatchAnime)
            .WithName("GetUserPlanToWatchAnime")
            .WithSummary("Gets plan to watch anime for a specific user from MyAnimeList")
            .WithDescription("Returns a list of plan to watch anime for the specified MyAnimeList user. Optional sort parameter: list_score, list_updated_at, anime_title, anime_start_date, anime_id")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);

        // Get User's On Hold Anime
        group.MapGet("/users/{username}/on-hold", GetUserOnHoldAnime)
            .WithName("GetUserOnHoldAnime")
            .WithSummary("Gets on hold anime for a specific user from MyAnimeList")
            .WithDescription("Returns a list of on hold anime for the specified MyAnimeList user. Optional sort parameter: list_score, list_updated_at, anime_title, anime_start_date, anime_id")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);

        // Get User's Dropped Anime
        group.MapGet("/users/{username}/dropped", GetUserDroppedAnime)
            .WithName("GetUserDroppedAnime")
            .WithSummary("Gets dropped anime for a specific user from MyAnimeList")
            .WithDescription("Returns a list of dropped anime for the specified MyAnimeList user. Optional sort parameter: list_score, list_updated_at, anime_title, anime_start_date, anime_id")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);

        // Get User's Anime List with Custom Status
        group.MapGet("/users/{username}/animelist", GetUserAnimeList)
            .WithName("GetUserAnimeList")
            .WithSummary("Gets anime list for a specific user with custom status filter")
            .WithDescription("Returns anime list for the specified user with optional status filter (watching, completed, on_hold, dropped, plan_to_watch) and sort options (list_score, list_updated_at, anime_title, anime_start_date, anime_id)")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(400)
            .Produces(401)
            .Produces(404)
            .Produces(500);
    }

    private static void MapAnimeEndpoints(RouteGroupBuilder group)
    {
        // Get Anime Details by ID
        group.MapGet("/anime/{animeId:int}", GetAnimeDetails)
            .WithName("GetAnimeDetails")
            .WithSummary("Gets detailed information about a specific anime")
            .WithDescription("Returns detailed information about an anime from the MyAnimeList API")
            .Produces<AnimeNode>(200)
            .Produces(400)
            .Produces(404)
            .Produces(500);

        // Search Anime
        group.MapGet("/anime/search", SearchAnime)
            .WithName("SearchAnime")
            .WithSummary("Search for anime by query")
            .WithDescription("Searches for anime using the MyAnimeList API with the specified query string")
            .Produces<MyAnimeListResponse<AnimeSearchResult>>(200)
            .Produces(400)
            .Produces(500);
    }

    private static void MapDiscoveryEndpoints(RouteGroupBuilder group)
    {
        // Get Anime Rankings
        group.MapGet("/anime/ranking", GetAnimeRanking)
            .WithName("GetAnimeRanking")
            .WithSummary("Gets anime rankings")
            .WithDescription("Returns anime rankings by type (all, airing, upcoming, tv, ova, movie, special, bypopularity, favorite)")
            .Produces<MyAnimeListResponse<AnimeRankingEntry>>(200)
            .Produces(400)
            .Produces(500);

        // Get Seasonal Anime
        group.MapGet("/anime/season/{year:int}/{season}", GetSeasonalAnime)
            .WithName("GetSeasonalAnime")
            .WithSummary("Gets seasonal anime")
            .WithDescription("Returns anime for the specified year and season (winter, spring, summer, fall)")
            .Produces<MyAnimeListResponse<AnimeSeasonEntry>>(200)
            .Produces(400)
            .Produces(500);

        // Get Suggested Anime
        group.MapGet("/anime/suggestions", GetSuggestedAnime)
            .WithName("GetSuggestedAnime")
            .WithSummary("Gets suggested anime")
            .WithDescription("Returns suggested anime from MyAnimeList (requires authentication)")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(401)
            .Produces(500);
    }

    // User anime list endpoint handlers
    private static async Task<IResult> GetUserCompletedAnime(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserCompletedAnimeAsync(username, sort, limit, offset, cancellationToken),
            username, "completed", logger);
    }

    private static async Task<IResult> GetUserCurrentlyWatchingAnime(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserCurrentlyWatchingAnimeAsync(username, sort, limit, offset, cancellationToken),
            username, "currently watching", logger);
    }

    private static async Task<IResult> GetUserPlanToWatchAnime(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserPlanToWatchAnimeAsync(username, sort, limit, offset, cancellationToken),
            username, "plan to watch", logger);
    }

    private static async Task<IResult> GetUserOnHoldAnime(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserOnHoldAnimeAsync(username, sort, limit, offset, cancellationToken),
            username, "on hold", logger);
    }

    private static async Task<IResult> GetUserDroppedAnime(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserDroppedAnimeAsync(username, sort, limit, offset, cancellationToken),
            username, "dropped", logger);
    }

    private static async Task<IResult> GetUserAnimeList(
        string username,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? status = null,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await HandleUserAnimeListRequest(
            () => myAnimeListService.GetUserAnimeListAsync(username, status, sort, limit, offset, cancellationToken),
            username, status ?? "all statuses", logger);
    }

    // Anime details and search endpoint handlers
    private static async Task<IResult> GetAnimeDetails(
        int animeId,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (animeId <= 0)
            {
                logger.LogWarning("Invalid anime ID provided: {AnimeId}", animeId);
                return Results.BadRequest("Anime ID must be a positive integer");
            }

            logger.LogInformation("Fetching anime details for ID: {AnimeId}", animeId);

            var result = await myAnimeListService.GetAnimeDetailsAsync(animeId, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("Anime not found for ID: {AnimeId}", animeId);
                return Results.NotFound($"Anime with ID '{animeId}' not found");
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request cancelled for anime ID: {AnimeId}", animeId);
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching anime details for ID: {AnimeId}", animeId);
            return Results.Problem("An error occurred while fetching anime details");
        }
    }

    private static async Task<IResult> SearchAnime(
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string query,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                logger.LogWarning("Empty or null search query provided");
                return Results.BadRequest("Search query cannot be empty");
            }

            logger.LogInformation("Searching anime with query: {Query}", query);

            var result = await myAnimeListService.SearchAnimeAsync(query, limit, offset, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("No search results found for query: {Query}", query);
                return Results.Ok(new MyAnimeListResponse<AnimeSearchResult>(
                    Data: Array.Empty<AnimeSearchResult>(), 
                    Paging: new Paging(Previous: null, Next: null)
                ));
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Search request cancelled for query: {Query}", query);
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error searching anime with query: {Query}", query);
            return Results.Problem("An error occurred while searching anime");
        }
    }

    // Discovery endpoint handlers
    private static async Task<IResult> GetAnimeRanking(
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string rankingType = "all",
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate ranking type
            var validTypes = new[] { "all", "airing", "upcoming", "tv", "ova", "movie", "special", "bypopularity", "favorite" };
            if (!validTypes.Contains(rankingType.ToLowerInvariant()))
            {
                logger.LogWarning("Invalid ranking type provided: {RankingType}", rankingType);
                return Results.BadRequest($"Invalid ranking type. Valid values: {string.Join(", ", validTypes)}");
            }

            logger.LogInformation("Fetching anime ranking with type: {RankingType}", rankingType);

            var result = await myAnimeListService.GetAnimeRankingAsync(rankingType, limit, offset, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("No ranking results found for type: {RankingType}", rankingType);
                return Results.Ok(new MyAnimeListResponse<AnimeRankingEntry>(
                    Data: Array.Empty<AnimeRankingEntry>(), 
                    Paging: new Paging(Previous: null, Next: null)
                ));
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Ranking request cancelled for type: {RankingType}", rankingType);
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching anime ranking with type: {RankingType}", rankingType);
            return Results.Problem("An error occurred while fetching anime ranking");
        }
    }

    private static async Task<IResult> GetSeasonalAnime(
        int year,
        string season,
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] string? sort = null,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate year
            if (year < 1917 || year > DateTime.UtcNow.Year + 2)
            {
                logger.LogWarning("Invalid year provided: {Year}", year);
                return Results.BadRequest("Year must be between 1917 and current year + 2");
            }

            // Validate season
            var validSeasons = new[] { "winter", "spring", "summer", "fall" };
            if (!validSeasons.Contains(season.ToLowerInvariant()))
            {
                logger.LogWarning("Invalid season provided: {Season}", season);
                return Results.BadRequest($"Invalid season. Valid values: {string.Join(", ", validSeasons)}");
            }

            logger.LogInformation("Fetching seasonal anime for {Year} {Season}", year, season);

            var result = await myAnimeListService.GetSeasonalAnimeAsync(year, season, sort, limit, offset, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("No seasonal anime found for {Year} {Season}", year, season);
                return Results.Ok(new MyAnimeListResponse<AnimeSeasonEntry>(
                    Data: Array.Empty<AnimeSeasonEntry>(), 
                    Paging: new Paging(Previous: null, Next: null)
                ));
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Seasonal anime request cancelled for {Year} {Season}", year, season);
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching seasonal anime for {Year} {Season}", year, season);
            return Results.Problem("An error occurred while fetching seasonal anime");
        }
    }

    private static async Task<IResult> GetSuggestedAnime(
        IMyAnimeListService myAnimeListService,
        ILogger<MyAnimeListService> logger,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Fetching suggested anime");

            var result = await myAnimeListService.GetSuggestedAnimeAsync(limit, offset, cancellationToken);

            if (result == null)
            {
                logger.LogWarning("No suggested anime found");
                return Results.Ok(new MyAnimeListResponse<AnimeEntry>(
                    Data: Array.Empty<AnimeEntry>(), 
                    Paging: new Paging(Previous: null, Next: null)
                ));
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Suggested anime request cancelled");
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching suggested anime");
            return Results.Problem("An error occurred while fetching suggested anime");
        }
    }

    // Helper method to reduce code duplication for user anime list endpoints
    private static async Task<IResult> HandleUserAnimeListRequest(
        Func<Task<MyAnimeListResponse<AnimeEntry>?>> serviceCall,
        string username,
        string listType,
        ILogger<MyAnimeListService> logger)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                logger.LogWarning("Empty or null username provided");
                return Results.BadRequest("Username cannot be empty");
            }

            logger.LogInformation("Fetching {ListType} anime for user: {Username}", listType, username);

            var result = await serviceCall();

            if (result == null)
            {
                logger.LogWarning("Failed to fetch {ListType} anime for user: {Username}", listType, username);
                return Results.NotFound($"User '{username}' not found or no {listType} anime");
            }

            return Results.Ok(result);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Request cancelled for {ListType} anime for user: {Username}", listType, username);
            return Results.StatusCode(499); // Client closed request
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error fetching {ListType} anime for user: {Username}", listType, username);
            return Results.Problem($"An error occurred while fetching {listType} anime");
        }
    }
}
