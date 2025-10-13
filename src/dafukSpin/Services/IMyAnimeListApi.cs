using dafukSpin.Models;
using Refit;

namespace dafukSpin.Services;

/// <summary>
/// Refit interface for MyAnimeList API v2 based on official documentation
/// https://myanimelist.net/apiconfig/references/api/v2
/// </summary>
public interface IMyAnimeListApi
{
    /// <summary>
    /// Get user anime list
    /// GET /v2/users/{user_name}/animelist
    /// </summary>
    /// <param name="userName">User name or @me</param>
    /// <param name="status">Filter by status: watching, completed, on_hold, dropped, plan_to_watch</param>
    /// <param name="sort">Sort order: list_score, list_updated_at, anime_title, anime_start_date, anime_id</param>
    /// <param name="limit">Maximum number of results (1-1000, default: 100)</param>
    /// <param name="offset">Offset for pagination (default: 0)</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's anime list response</returns>
    [Get("/users/{userName}/animelist")]
    Task<MyAnimeListResponse<AnimeEntry>> GetUserAnimeListAsync(
        string userName,
        [Query] string? status = null,
        [Query] string? sort = null,
        [Query] int limit = 100,
        [Query] int offset = 0,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get anime details by ID
    /// GET /v2/anime/{anime_id}
    /// </summary>
    /// <param name="animeId">Anime ID</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Anime details</returns>
    [Get("/anime/{animeId}")]
    Task<AnimeNode> GetAnimeDetailsAsync(
        int animeId,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search anime
    /// GET /v2/anime
    /// </summary>
    /// <param name="query">Search query</param>
    /// <param name="limit">Maximum number of results (1-100, default: 100)</param>
    /// <param name="offset">Offset for pagination (default: 0)</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Search results</returns>
    [Get("/anime")]
    Task<MyAnimeListResponse<AnimeSearchResult>> SearchAnimeAsync(
        [Query] string query,
        [Query] int limit = 100,
        [Query] int offset = 0,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get anime ranking
    /// GET /v2/anime/ranking
    /// </summary>
    /// <param name="rankingType">Ranking type: all, airing, upcoming, tv, ova, movie, special, bypopularity, favorite</param>
    /// <param name="limit">Maximum number of results (1-500, default: 100)</param>
    /// <param name="offset">Offset for pagination (default: 0)</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Anime ranking response</returns>
    [Get("/anime/ranking")]
    Task<MyAnimeListResponse<AnimeRankingEntry>> GetAnimeRankingAsync(
        [Query("ranking_type")] string rankingType,
        [Query] int limit = 100,
        [Query] int offset = 0,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get seasonal anime
    /// GET /v2/anime/season/{year}/{season}
    /// </summary>
    /// <param name="year">Year (1917 or later)</param>
    /// <param name="season">Season: winter, spring, summer, fall</param>
    /// <param name="sort">Sort order: anime_score, anime_num_list_users</param>
    /// <param name="limit">Maximum number of results (1-500, default: 100)</param>
    /// <param name="offset">Offset for pagination (default: 0)</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Seasonal anime response</returns>
    [Get("/anime/season/{year}/{season}")]
    Task<MyAnimeListResponse<AnimeSeasonEntry>> GetSeasonalAnimeAsync(
        int year,
        string season,
        [Query] string? sort = null,
        [Query] int limit = 100,
        [Query] int offset = 0,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get suggested anime
    /// GET /v2/anime/suggestions
    /// </summary>
    /// <param name="limit">Maximum number of results (1-100, default: 100)</param>
    /// <param name="offset">Offset for pagination (default: 0)</param>
    /// <param name="fields">Comma-separated list of fields to include</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Suggested anime response</returns>
    [Get("/anime/suggestions")]
    Task<MyAnimeListResponse<AnimeEntry>> GetSuggestedAnimeAsync(
        [Query] int limit = 100,
        [Query] int offset = 0,
        [Query] string? fields = null,
        CancellationToken cancellationToken = default);
}