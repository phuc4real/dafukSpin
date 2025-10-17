using dafukSpin.Endpoints;
using dafukSpin.Extensions;
using dafukSpin.Models;
using dafukSpin.Services;
using Microsoft.AspNetCore.Mvc;

namespace dafukSpin.Endpoints;

/// <summary>
/// Endpoints for anime details and search operations
/// </summary>
public sealed class AnimeDataEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/anime")
            .WithTags("Anime Data");

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
        IPaginationUrlRewriteService paginationRewriteService,
        HttpContext httpContext,
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
            if (result is not null)
            {
                var rewrittenResult = result.RewritePaginationUrls(paginationRewriteService, httpContext, "/api/anime/search");
                return Results.Ok(rewrittenResult);
            }
            return Results.NotFound($"No anime found for query '{query}'");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error searching anime with query '{query}': {ex.Message}", statusCode: 500);
        }
    }
}