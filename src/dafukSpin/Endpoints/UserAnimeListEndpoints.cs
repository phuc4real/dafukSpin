using dafukSpin.Endpoints;
using dafukSpin.Models;
using dafukSpin.Services;
using Microsoft.AspNetCore.Mvc;

namespace dafukSpin.Endpoints;

/// <summary>
/// Endpoints for user anime list operations
/// </summary>
public sealed class UserAnimeListEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/anime/users/{username}")
            .WithTags("User Anime Lists");

        group.MapGet("/anime", GetUserAnimeListAsync)
            .WithName("GetUserAnimeList")
            .WithSummary("Get user's anime list")
            .WithDescription("Retrieves a user's anime list with optional filtering by status and sorting")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/anime/completed", GetUserCompletedAnimeAsync)
            .WithName("GetUserCompletedAnime")
            .WithSummary("Get user's completed anime")
            .WithDescription("Retrieves anime that the user has completed watching")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/anime/watching", GetUserCurrentlyWatchingAnimeAsync)
            .WithName("GetUserCurrentlyWatchingAnime")
            .WithSummary("Get user's currently watching anime")
            .WithDescription("Retrieves anime that the user is currently watching")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/anime/plan-to-watch", GetUserPlanToWatchAnimeAsync)
            .WithName("GetUserPlanToWatchAnime")
            .WithSummary("Get user's plan to watch anime")
            .WithDescription("Retrieves anime that the user plans to watch")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/anime/on-hold", GetUserOnHoldAnimeAsync)
            .WithName("GetUserOnHoldAnime")
            .WithSummary("Get user's on-hold anime")
            .WithDescription("Retrieves anime that the user has put on hold")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);

        group.MapGet("/anime/dropped", GetUserDroppedAnimeAsync)
            .WithName("GetUserDroppedAnime")
            .WithSummary("Get user's dropped anime")
            .WithDescription("Retrieves anime that the user has dropped")
            .Produces<MyAnimeListResponse<AnimeEntry>>(200)
            .Produces(404)
            .Produces(500);
    }

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
}