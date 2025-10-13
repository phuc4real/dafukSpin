using dafukSpin.Models;
using Refit;

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

public sealed class MyAnimeListService : IMyAnimeListService
{
    private readonly IMyAnimeListApi _api;
    private readonly ILogger<MyAnimeListService> _logger;
    
    // Standard field sets for different operations
    private const string DetailedFields = "alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,broadcast,source,average_episode_duration,rating,pictures,background,related_anime,related_manga,recommendations,studios,statistics,genres,media_type,status,num_list_users,num_scoring_users,nsfw,created_at,updated_at";
    private const string ListFields = "list_status,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,broadcast,source,average_episode_duration,rating,pictures,background,genres,studios,media_type,status";
    private const string BasicFields = "alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,start_season,source,rating,genres,studios,media_type";

    public MyAnimeListService(IMyAnimeListApi api, ILogger<MyAnimeListService> logger)
    {
        _api = api;
        _logger = logger;
    }

    public async Task<MyAnimeListResponse<AnimeEntry>?> GetUserAnimeListAsync(
        string username, 
        string? status = null, 
        string? sort = null, 
        int limit = 100, 
        int offset = 0, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Fetching anime list for user: {Username} with status: {Status}, sort: {Sort}", username, status, sort);
            
            var result = await _api.GetUserAnimeListAsync(username, status, sort, limit, offset, ListFields, cancellationToken);
            
            _logger.LogInformation("Successfully fetched {Count} anime entries for user: {Username}", result.Data.Count, username);
            return result;
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("API error fetching anime list for user: {Username}. Status: {StatusCode}, Content: {Content}", 
                username, ex.StatusCode, ex.Content);
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
        try
        {
            _logger.LogInformation("Fetching anime details for ID: {AnimeId}", animeId);
            
            var result = await _api.GetAnimeDetailsAsync(animeId, DetailedFields, cancellationToken);
            
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
        try
        {
            _logger.LogInformation("Searching anime with query: {Query}", query);
            
            var result = await _api.SearchAnimeAsync(query, limit, offset, BasicFields, cancellationToken);
            
            _logger.LogInformation("Successfully found {Count} anime results for query: {Query}", result.Data.Count, query);
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
        try
        {
            _logger.LogInformation("Fetching anime ranking with type: {RankingType}", rankingType);
            
            var result = await _api.GetAnimeRankingAsync(rankingType, limit, offset, BasicFields, cancellationToken);
            
            _logger.LogInformation("Successfully fetched {Count} anime ranking entries for type: {RankingType}", result.Data.Count, rankingType);
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
        try
        {
            _logger.LogInformation("Fetching seasonal anime for {Year} {Season} with sort: {Sort}", year, season, sort);
            
            var result = await _api.GetSeasonalAnimeAsync(year, season, sort, limit, offset, BasicFields, cancellationToken);
            
            _logger.LogInformation("Successfully fetched {Count} seasonal anime entries for {Year} {Season}", result.Data.Count, year, season);
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
        try
        {
            _logger.LogInformation("Fetching suggested anime");
            
            var result = await _api.GetSuggestedAnimeAsync(limit, offset, BasicFields, cancellationToken);
            
            _logger.LogInformation("Successfully fetched {Count} suggested anime entries", result.Data.Count);
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