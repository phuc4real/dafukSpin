using dafukSpin.Models;

namespace dafukSpin.Services;

/// <summary>
/// Service wrapper for MyAnimeList API using Refit
/// Provides high-level business operations with error handling and logging
/// </summary>
public interface IMyAnimeListService
{
    // User anime list operations
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserAnimeListAsync(string username, string? status = null, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserCompletedAnimeAsync(string username, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserCurrentlyWatchingAnimeAsync(string username, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserPlanToWatchAnimeAsync(string username, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserOnHoldAnimeAsync(string username, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetUserDroppedAnimeAsync(string username, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);

    // Anime details and search
    Task<AnimeNode?> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeSearchResult>?> SearchAnimeAsync(string query, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);

    // Rankings and seasonal
    Task<MyAnimeListResponse<AnimeRankingEntry>?> GetAnimeRankingAsync(string rankingType = "all", int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeSeasonEntry>?> GetSeasonalAnimeAsync(int year, string season, string? sort = null, int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
    Task<MyAnimeListResponse<AnimeEntry>?> GetSuggestedAnimeAsync(int limit = 100, int offset = 0, CancellationToken cancellationToken = default);
}