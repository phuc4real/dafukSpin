using System.Text.Json;
using dafukSpin.Models;

namespace dafukSpin.Services
{
    public interface IMyAnimeListService
    {
        Task<List<CompletedAnimeDto>> GetTop10LatestCompletedAnimeAsync(string username);
        Task<List<CurrentlyWatchingAnimeDto>> GetTop5CurrentlyWatchingAnimeAsync(string username);
        Task<PagedAnimeHistoryResponse> GetUserAnimeHistoryAsync(string username, int page = 1, int pageSize = 20);
        Task<PlanToWatchAnimeDto?> GetRandomPlanToWatchAnimeAsync(string username);
    }

    public class MyAnimeListService : IMyAnimeListService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MyAnimeListService> _logger;
        private readonly IConfiguration _configuration;
        private const string BaseUrl = "https://api.myanimelist.net/v2";

        public MyAnimeListService(HttpClient httpClient, ILogger<MyAnimeListService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            
            // Configure HttpClient
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            
            // Add Client ID for MyAnimeList API (required for public endpoints)
            var clientId = _configuration["MyAnimeList:ClientId"];
            if (!string.IsNullOrEmpty(clientId))
            {
                _httpClient.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", clientId);
            }
        }

        public async Task<List<CompletedAnimeDto>> GetTop10LatestCompletedAnimeAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching completed anime for user: {Username}", username);

                var animeList = await GetUserAnimeListAsync(username, "completed", 10, "list_updated_at");

                var completedAnime = animeList
                    .Where(entry => entry.ListStatus.Status == "completed")
                    .OrderByDescending(entry => entry.ListStatus.UpdatedAt)
                    .Take(10)
                    .Select(entry => new CompletedAnimeDto
                    {
                        Id = entry.Node.Id,
                        Title = entry.Node.Title,
                        EnglishTitle = entry.Node.AlternativeTitles.English,
                        ImageUrl = entry.Node.MainPicture.Large ?? entry.Node.MainPicture.Medium,
                        Score = entry.Node.Mean,
                        UserScore = entry.ListStatus.Score,
                        Rank = entry.Node.Rank,
                        Popularity = entry.Node.Popularity,
                        NumEpisodes = entry.Node.NumEpisodes,
                        MediaType = entry.Node.MediaType,
                        Rating = entry.Node.Rating,
                        Genres = entry.Node.Genres.Select(g => g.Name).ToList(),
                        CompletedAt = entry.ListStatus.UpdatedAt,
                        FinishDate = entry.ListStatus.FinishDate
                    })
                    .ToList();

                _logger.LogInformation("Successfully fetched {Count} completed anime for user {Username}", 
                    completedAnime.Count, username);

                return completedAnime;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching completed anime for user {Username}", username);
                throw;
            }
        }

        public async Task<List<CurrentlyWatchingAnimeDto>> GetTop5CurrentlyWatchingAnimeAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching currently watching anime for user: {Username}", username);

                var animeList = await GetUserAnimeListAsync(username, "watching", 5, "list_updated_at");

                var currentlyWatching = animeList
                    .Where(entry => entry.ListStatus.Status == "watching")
                    .OrderByDescending(entry => entry.ListStatus.UpdatedAt)
                    .Take(5)
                    .Select(entry => new CurrentlyWatchingAnimeDto
                    {
                        Id = entry.Node.Id,
                        Title = entry.Node.Title,
                        EnglishTitle = entry.Node.AlternativeTitles.English,
                        ImageUrl = entry.Node.MainPicture.Large ?? entry.Node.MainPicture.Medium,
                        Score = entry.Node.Mean,
                        UserScore = entry.ListStatus.Score,
                        Rank = entry.Node.Rank,
                        Popularity = entry.Node.Popularity,
                        NumEpisodes = entry.Node.NumEpisodes,
                        NumEpisodesWatched = entry.ListStatus.NumEpisodesWatched,
                        MediaType = entry.Node.MediaType,
                        Rating = entry.Node.Rating,
                        Genres = entry.Node.Genres.Select(g => g.Name).ToList(),
                        Studios = entry.Node.Studios.Select(s => s.Name).ToList(),
                        IsRewatching = entry.ListStatus.IsRewatching,
                        LastUpdated = entry.ListStatus.UpdatedAt,
                        StartDate = entry.ListStatus.StartDate,
                        ProgressPercentage = CalculateProgressPercentage(entry.ListStatus.NumEpisodesWatched, entry.Node.NumEpisodes)
                    })
                    .ToList();

                _logger.LogInformation("Successfully fetched {Count} currently watching anime for user {Username}", 
                    currentlyWatching.Count, username);

                return currentlyWatching;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching currently watching anime for user {Username}", username);
                throw;
            }
        }

        public async Task<PagedAnimeHistoryResponse> GetUserAnimeHistoryAsync(string username, int page = 1, int pageSize = 20)
        {
            try
            {
                _logger.LogInformation("Fetching anime history for user: {Username}, page: {Page}, pageSize: {PageSize}", 
                    username, page, pageSize);

                // Get all statuses for comprehensive history
                var allAnimeList = await GetUserAnimeListAsync(username, "", pageSize * 5, "list_updated_at"); // Get more data for pagination

                var allHistory = allAnimeList
                    .OrderByDescending(entry => entry.ListStatus.UpdatedAt)
                    .Select(entry => new AnimeHistoryDto
                    {
                        Id = entry.Node.Id,
                        Title = entry.Node.Title,
                        EnglishTitle = entry.Node.AlternativeTitles.English,
                        ImageUrl = entry.Node.MainPicture.Large ?? entry.Node.MainPicture.Medium,
                        Score = entry.Node.Mean,
                        UserScore = entry.ListStatus.Score,
                        Rank = entry.Node.Rank,
                        Popularity = entry.Node.Popularity,
                        NumEpisodes = entry.Node.NumEpisodes,
                        MediaType = entry.Node.MediaType,
                        Rating = entry.Node.Rating,
                        Genres = entry.Node.Genres.Select(g => g.Name).ToList(),
                        Studios = entry.Node.Studios.Select(s => s.Name).ToList(),
                        CompletedAt = entry.ListStatus.UpdatedAt,
                        FinishDate = entry.ListStatus.FinishDate,
                        StartDate = entry.ListStatus.StartDate,
                        NumTimesRewatched = entry.ListStatus.NumTimesRewatched,
                        Comments = entry.ListStatus.Comments,
                        Tags = entry.ListStatus.Tags
                    })
                    .ToList();

                var totalItems = allHistory.Count;
                var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                var skip = (page - 1) * pageSize;
                var pagedData = allHistory.Skip(skip).Take(pageSize).ToList();

                var response = new PagedAnimeHistoryResponse
                {
                    Data = pagedData,
                    Pagination = new PaginationInfo
                    {
                        CurrentPage = page,
                        TotalPages = totalPages,
                        TotalItems = totalItems,
                        PageSize = pageSize,
                        HasNext = page < totalPages,
                        HasPrevious = page > 1
                    }
                };

                _logger.LogInformation("Successfully fetched anime history for user {Username}. Page {Page}/{TotalPages}, {Count} items", 
                    username, page, totalPages, pagedData.Count);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching anime history for user {Username}", username);
                throw;
            }
        }

        public async Task<PlanToWatchAnimeDto?> GetRandomPlanToWatchAnimeAsync(string username)
        {
            try
            {
                _logger.LogInformation("Fetching random plan to watch anime for user: {Username}", username);

                var planToWatchList = await GetUserAnimeListAsync(username, "plan_to_watch", 1000, "list_updated_at");

                var planToWatchAnime = planToWatchList
                    .Where(entry => entry.ListStatus.Status == "plan_to_watch")
                    .ToList();

                if (!planToWatchAnime.Any())
                {
                    _logger.LogInformation("No plan to watch anime found for user {Username}", username);
                    return null;
                }

                // Get random anime from the list
                var random = new Random();
                var randomEntry = planToWatchAnime[random.Next(planToWatchAnime.Count)];

                var result = new PlanToWatchAnimeDto
                {
                    Id = randomEntry.Node.Id,
                    Title = randomEntry.Node.Title,
                    EnglishTitle = randomEntry.Node.AlternativeTitles.English,
                    ImageUrl = randomEntry.Node.MainPicture.Large ?? randomEntry.Node.MainPicture.Medium,
                    Score = randomEntry.Node.Mean,
                    Rank = randomEntry.Node.Rank,
                    Popularity = randomEntry.Node.Popularity,
                    NumEpisodes = randomEntry.Node.NumEpisodes,
                    MediaType = randomEntry.Node.MediaType,
                    Rating = randomEntry.Node.Rating,
                    Genres = randomEntry.Node.Genres.Select(g => g.Name).ToList(),
                    Studios = randomEntry.Node.Studios.Select(s => s.Name).ToList(),
                    Synopsis = randomEntry.Node.Synopsis,
                    StartDate = randomEntry.Node.StartDate,
                    EndDate = randomEntry.Node.EndDate,
                    Source = randomEntry.Node.Source,
                    Priority = randomEntry.ListStatus.Priority,
                    AddedToListAt = randomEntry.ListStatus.UpdatedAt,
                    Tags = randomEntry.ListStatus.Tags,
                    Comments = randomEntry.ListStatus.Comments
                };

                _logger.LogInformation("Successfully selected random plan to watch anime '{Title}' for user {Username}", 
                    result.Title, username);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching random plan to watch anime for user {Username}", username);
                throw;
            }
        }

        private async Task<List<AnimeEntry>> GetUserAnimeListAsync(string username, string status, int limit, string sort)
        {
            // Check if MyAnimeList Client ID is configured
            var clientId = _configuration["MyAnimeList:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new InvalidOperationException("MyAnimeList Client ID is not configured. Please add 'MyAnimeList:ClientId' to your configuration.");
            }

            // Build request URL
            var requestUrl = $"/users/{username}/animelist?";
            
            if (!string.IsNullOrEmpty(status))
            {
                requestUrl += $"status={status}&";
            }
            
            requestUrl += $"sort={sort}&" +
                         $"limit={limit}&" +
                         "fields=id,title,main_picture,alternative_titles,start_date,end_date,synopsis,mean,rank,popularity,num_episodes,status,genres,media_type,rating,source,studios";

            var response = await _httpClient.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Failed to fetch data from MyAnimeList API. Status: {StatusCode}, Reason: {ReasonPhrase}", 
                    response.StatusCode, response.ReasonPhrase);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    throw new ArgumentException($"User '{username}' not found or has private list");
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Invalid MyAnimeList Client ID or insufficient permissions");
                }
                
                throw new HttpRequestException($"MyAnimeList API request failed: {response.StatusCode}");
            }

            var jsonContent = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("Received response: {JsonContent}", jsonContent);

            var malResponse = JsonSerializer.Deserialize<MyAnimeListResponse>(jsonContent);

            if (malResponse?.Data == null)
            {
                _logger.LogWarning("No data received from MyAnimeList API");
                return new List<AnimeEntry>();
            }

            return malResponse.Data;
        }

        private static double CalculateProgressPercentage(int episodesWatched, int? totalEpisodes)
        {
            if (!totalEpisodes.HasValue || totalEpisodes <= 0)
            {
                return 0.0;
            }

            return Math.Min(100.0, (double)episodesWatched / totalEpisodes.Value * 100.0);
        }
    }
}