using dafukSpin.Endpoints;
using dafukSpin.Extensions;
using dafukSpin.Models;
using dafukSpin.Services;
using Microsoft.AspNetCore.Mvc;

namespace dafukSpin.Endpoints;

/// <summary>
/// Endpoints for anime rankings and seasonal data
/// </summary>
public sealed class AnimeRankingEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/anime")
            .WithTags("Anime Rankings & Seasonal");

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
    }

    private static async Task<IResult> GetAnimeRankingAsync(
        IMyAnimeListService service,
        IPaginationUrlRewriteService paginationRewriteService,
        HttpContext httpContext,
        [FromQuery] string rankingType = "all",
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetAnimeRankingAsync(rankingType, limit, offset, cancellationToken);
            if (result is not null)
            {
                var rewrittenResult = result.RewritePaginationUrls(paginationRewriteService, httpContext, "/api/anime/ranking");
                return Results.Ok(rewrittenResult);
            }
            return Results.NotFound($"No ranking data found for type '{rankingType}'");
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
        IPaginationUrlRewriteService paginationRewriteService,
        HttpContext httpContext,
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
            if (result is not null)
            {
                var rewrittenResult = result.RewritePaginationUrls(paginationRewriteService, httpContext, $"/api/anime/seasonal/{year}/{season}");
                return Results.Ok(rewrittenResult);
            }
            return Results.NotFound($"No seasonal anime found for {season} {year}");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving seasonal anime for {season} {year}: {ex.Message}", statusCode: 500);
        }
    }

    private static async Task<IResult> GetSuggestedAnimeAsync(
        IMyAnimeListService service,
        IPaginationUrlRewriteService paginationRewriteService,
        HttpContext httpContext,
        [FromQuery] int limit = 100,
        [FromQuery] int offset = 0,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await service.GetSuggestedAnimeAsync(limit, offset, cancellationToken);
            if (result is not null)
            {
                var rewrittenResult = result.RewritePaginationUrls(paginationRewriteService, httpContext, "/api/anime/suggested");
                return Results.Ok(rewrittenResult);
            }
            return Results.NotFound("No suggested anime found");
        }
        catch (Exception ex)
        {
            return Results.Problem($"Error retrieving suggested anime: {ex.Message}", statusCode: 500);
        }
    }
}