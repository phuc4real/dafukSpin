using dafukSpin.Models;
using dafukSpin.Services.Caching;
using dafukSpin.Extensions;
using Refit;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace dafukSpin.Services;

/// <summary>
/// Service implementation for MyAnimeList API using Refit
/// Provides high-level business operations with error handling and logging
/// </summary>
public sealed class MyAnimeListService : IMyAnimeListService
{
    private readonly IMyAnimeListApi _api;
    private readonly ILogger<MyAnimeListService> _logger;
    private readonly ICacheService _cache;

    // Standard field sets for different operations
    private const string DetailedFields = "alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,broadcast,source,average_episode_duration,rating,pictures,background,related_anime,related_manga,recommendations,studios,statistics,genres,media_type,status,num_list_users,num_scoring_users,nsfw,created_at,updated_at";
    private const string ListFields = "list_status,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,broadcast,source,average_episode_duration,rating,pictures,background,genres,studios,media_type,status";
    private const string BasicFields = "alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,source,rating,genres,studios,media_type";

    // Cache expiration times
    private static readonly TimeSpan AnimeDetailsCacheExpiry = TimeSpan.FromHours(6); // Anime details change rarely
    private static readonly TimeSpan UserListCacheExpiry = TimeSpan.FromMinutes(15); // User lists change more frequently
    private static readonly TimeSpan SearchCacheExpiry = TimeSpan.FromMinutes(30); // Search results can be cached for moderate time
    private static readonly TimeSpan RankingCacheExpiry = TimeSpan.FromHours(2); // Rankings update less frequently
    private static readonly TimeSpan SeasonalCacheExpiry = TimeSpan.FromHours(4); // Seasonal data is relatively static
    private static readonly TimeSpan SuggestedCacheExpiry = TimeSpan.FromMinutes(10); // Suggestions might be more personalized

    public MyAnimeListService(IMyAnimeListApi api, ILogger<MyAnimeListService> logger, ICacheService cache)
    {
        _api = api;
        _logger = logger;
        _cache = cache;
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserAnimeListAsync(
        string username,
        string? status = null,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        using var activity = ObservabilityExtensions.StartActivity("MyAnimeListService.GetUserAnimeList");

        // Add telemetry tags
        ObservabilityExtensions.AddTagToCurrentActivity("mal.username", username);
        ObservabilityExtensions.AddTagToCurrentActivity("mal.status", status);
        ObservabilityExtensions.AddTagToCurrentActivity("mal.sort", sort);
        ObservabilityExtensions.AddTagToCurrentActivity("mal.limit", limit);
        ObservabilityExtensions.AddTagToCurrentActivity("mal.offset", offset);

        // Create cache key based on all parameters
        var cacheKey = $"user_anime_list:{username}:{status}:{sort}:{limit}:{offset}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<MyAnimeListResponse<AnimeEntry>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for user anime list: {CacheKey} for {Username}", cacheKey, username);
            ObservabilityExtensions.AddTagToCurrentActivity("cache.hit", true);
            ObservabilityExtensions.AddTagToCurrentActivity("cache.key", cacheKey);
            return cachedResult;
        }

        ObservabilityExtensions.AddTagToCurrentActivity("cache.hit", false);

        try
        {
            _logger.LogInformation("Fetching anime list for user: {Username} with status: {Status}, sort: {Sort}", username, status, sort);

            var result = await _api.GetUserAnimeListAsync(username, status, sort, limit, offset, ListFields, cancellationToken);

            if (result != null)
            {
                // Cache the result with appropriate expiration
                await _cache.SetAsync(cacheKey, result, UserListCacheExpiry, CacheItemPriority.Normal, cancellationToken);
                _logger.LogDebug("Cached user anime list: {CacheKey} for {Username}", cacheKey, username);

                ObservabilityExtensions.AddTagToCurrentActivity("result.count", result.Data.Count);
                ObservabilityExtensions.AddTagToCurrentActivity("result.has_next", result.Paging?.Next != null);
            }

            _logger.LogInformation("Successfully fetched {Count} anime entries for user: {Username}", result?.Data.Count ?? 0, username);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching anime list for user: {Username}. Status: {StatusCode}, Content: {Content}",
                username, ex.StatusCode, ex.Content);

            ObservabilityExtensions.RecordException(ex);
            ObservabilityExtensions.AddTagToCurrentActivity("error.type", "ApiException");
            ObservabilityExtensions.AddTagToCurrentActivity("error.status_code", ex.StatusCode.ToString());

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching anime list for user: {Username}", username);
            return null;
        }
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserCompletedAnimeAsync(
        string username,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAnimeListAsync(username, "completed", sort, limit, offset, cancellationToken);
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserCurrentlyWatchingAnimeAsync(
        string username,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAnimeListAsync(username, "watching", sort, limit, offset, cancellationToken);
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserPlanToWatchAnimeAsync(
        string username,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAnimeListAsync(username, "plan_to_watch", sort, limit, offset, cancellationToken);
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserOnHoldAnimeAsync(
        string username,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAnimeListAsync(username, "on_hold", sort, limit, offset, cancellationToken);
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserDroppedAnimeAsync(
        string username,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return await GetUserAnimeListAsync(username, "dropped", sort, limit, offset, cancellationToken);
    }

    public async Task<AnimeNode?> GetAnimeDetailsAsync(int animeId, CancellationToken cancellationToken = default)
    {
        // Create cache key for anime details
        var cacheKey = $"anime_details:{animeId}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<AnimeNode>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for anime details: {CacheKey}", cacheKey);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Fetching anime details for ID: {AnimeId}", animeId);

            var result = await _api.GetAnimeDetailsAsync(animeId, DetailedFields, cancellationToken);

            if (result != null)
            {
                // Cache the result with longer expiration since anime details change rarely
                await _cache.SetAsync(cacheKey, result, AnimeDetailsCacheExpiry, CacheItemPriority.High, cancellationToken);
                _logger.LogDebug("Cached anime details: {CacheKey}", cacheKey);
            }

            _logger.LogInformation("Successfully fetched anime details for ID: {AnimeId}", animeId);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching anime details for ID: {AnimeId}. Status: {StatusCode}, Content: {Content}",
                animeId, ex.StatusCode, ex.Content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching anime details for ID: {AnimeId}", animeId);
            return null;
        }
    }

    public async Task<MyAnimeListResponse<AnimeSearchResult>?> SearchAnimeAsync(
        string query,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        // Create cache key for search results
        var cacheKey = $"anime_search:{query.ToLowerInvariant()}:{limit}:{offset}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<MyAnimeListResponse<AnimeSearchResult>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for anime search: {CacheKey}", cacheKey);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Searching anime with query: {Query}", query);

            var result = await _api.SearchAnimeAsync(query, limit, offset, BasicFields, cancellationToken);

            if (result != null)
            {
                // Cache search results with moderate expiration
                await _cache.SetAsync(cacheKey, result, SearchCacheExpiry, CacheItemPriority.Normal, cancellationToken);
                _logger.LogDebug("Cached anime search results: {CacheKey}", cacheKey);
            }

            _logger.LogInformation("Successfully found {Count} anime results for query: {Query}", result?.Data.Count ?? 0, query);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error searching anime with query: {Query}. Status: {StatusCode}, Content: {Content}",
                query, ex.StatusCode, ex.Content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching anime with query: {Query}", query);
            return null;
        }
    }

    public async Task<MyAnimeListResponse<AnimeRankingEntry>?> GetAnimeRankingAsync(
        string rankingType = "all",
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        // Create cache key for ranking results
        var cacheKey = $"anime_ranking:{rankingType}:{limit}:{offset}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<MyAnimeListResponse<AnimeRankingEntry>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for anime ranking: {CacheKey}", cacheKey);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Fetching anime ranking with type: {RankingType}", rankingType);

            var result = await _api.GetAnimeRankingAsync(rankingType, limit, offset, BasicFields, cancellationToken);

            if (result != null)
            {
                // Cache ranking results with longer expiration since they change less frequently
                await _cache.SetAsync(cacheKey, result, RankingCacheExpiry, CacheItemPriority.Normal, cancellationToken);
                _logger.LogDebug("Cached anime ranking: {CacheKey}", cacheKey);
            }

            _logger.LogInformation("Successfully fetched {Count} anime ranking entries for type: {RankingType}", result?.Data.Count ?? 0, rankingType);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching anime ranking with type: {RankingType}. Status: {StatusCode}, Content: {Content}",
                rankingType, ex.StatusCode, ex.Content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching anime ranking with type: {RankingType}", rankingType);
            return null;
        }
    }

    public async Task<MyAnimeListResponse<AnimeSeasonEntry>?> GetSeasonalAnimeAsync(
        int year,
        string season,
        string? sort = null,
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        // Create cache key for seasonal results
        var cacheKey = $"anime_seasonal:{year}:{season.ToLowerInvariant()}:{sort}:{limit}:{offset}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<MyAnimeListResponse<AnimeSeasonEntry>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for seasonal anime: {CacheKey}", cacheKey);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Fetching seasonal anime for {Year} {Season} with sort: {Sort}", year, season, sort);

            var result = await _api.GetSeasonalAnimeAsync(year, season, sort, limit, offset, BasicFields, cancellationToken);

            if (result != null)
            {
                // Cache seasonal results with longer expiration since they're relatively static
                await _cache.SetAsync(cacheKey, result, SeasonalCacheExpiry, CacheItemPriority.Normal, cancellationToken);
                _logger.LogDebug("Cached seasonal anime: {CacheKey}", cacheKey);
            }

            _logger.LogInformation("Successfully fetched {Count} seasonal anime entries for {Year} {Season}", result?.Data.Count ?? 0, year, season);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching seasonal anime for {Year} {Season}. Status: {StatusCode}, Content: {Content}",
                year, season, ex.StatusCode, ex.Content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching seasonal anime for {Year} {Season}", year, season);
            return null;
        }
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetSuggestedAnimeAsync(
        int limit = 100,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        // Create cache key for suggested anime
        var cacheKey = $"anime_suggested:{limit}:{offset}";

        // Try to get from cache first
        var cachedResult = await _cache.GetAsync<MyAnimeListResponse<AnimeEntry>>(cacheKey, cancellationToken);
        if (cachedResult != null)
        {
            _logger.LogDebug("Cache hit for suggested anime: {CacheKey}", cacheKey);
            return cachedResult;
        }

        try
        {
            _logger.LogInformation("Fetching suggested anime");

            var result = await _api.GetSuggestedAnimeAsync(limit, offset, BasicFields, cancellationToken);

            if (result != null)
            {
                // Cache suggested anime with shorter expiration since they might be more personalized
                await _cache.SetAsync(cacheKey, result, SuggestedCacheExpiry, CacheItemPriority.Low, cancellationToken);
                _logger.LogDebug("Cached suggested anime: {CacheKey}", cacheKey);
            }

            _logger.LogInformation("Successfully fetched {Count} suggested anime entries", result?.Data.Count ?? 0);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching suggested anime. Status: {StatusCode}, Content: {Content}",
                ex.StatusCode, ex.Content);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching suggested anime");
            return null;
        }
    }
}