using System.Text.Json.Serialization;

namespace dafukSpin.Models
{
    public class MyAnimeListResponse
    {
        [JsonPropertyName("data")]
        public List<AnimeEntry> Data { get; set; } = new();

        [JsonPropertyName("paging")]
        public Paging Paging { get; set; } = new();
    }

    public class Paging
    {
        [JsonPropertyName("previous")]
        public string Previous { get; set; } = string.Empty;

        [JsonPropertyName("next")]
        public string Next { get; set; } = string.Empty;
    }

    public class AnimeEntry
    {
        [JsonPropertyName("node")]
        public AnimeNode Node { get; set; } = new();

        [JsonPropertyName("list_status")]
        public ListStatus ListStatus { get; set; } = new();
    }

    public class AnimeNode
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("main_picture")]
        public Picture MainPicture { get; set; } = new();

        [JsonPropertyName("alternative_titles")]
        public AlternativeTitles AlternativeTitles { get; set; } = new();

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; } = string.Empty;

        [JsonPropertyName("synopsis")]
        public string Synopsis { get; set; } = string.Empty;

        [JsonPropertyName("mean")]
        public double? Mean { get; set; }

        [JsonPropertyName("rank")]
        public int? Rank { get; set; }

        [JsonPropertyName("popularity")]
        public int? Popularity { get; set; }

        [JsonPropertyName("num_episodes")]
        public int? NumEpisodes { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; } = new();

        [JsonPropertyName("media_type")]
        public string MediaType { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public string Rating { get; set; } = string.Empty;

        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("studios")]
        public List<Studio> Studios { get; set; } = new();
    }

    public class Picture
    {
        [JsonPropertyName("medium")]
        public string Medium { get; set; } = string.Empty;

        [JsonPropertyName("large")]
        public string Large { get; set; } = string.Empty;
    }

    public class AlternativeTitles
    {
        [JsonPropertyName("synonyms")]
        public List<string> Synonyms { get; set; } = new();

        [JsonPropertyName("en")]
        public string English { get; set; } = string.Empty;

        [JsonPropertyName("ja")]
        public string Japanese { get; set; } = string.Empty;
    }

    public class Genre
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class Studio
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
    }

    public class ListStatus
    {
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public int Score { get; set; }

        [JsonPropertyName("num_episodes_watched")]
        public int NumEpisodesWatched { get; set; }

        [JsonPropertyName("is_rewatching")]
        public bool IsRewatching { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = string.Empty;

        [JsonPropertyName("finish_date")]
        public string FinishDate { get; set; } = string.Empty;

        [JsonPropertyName("priority")]
        public int Priority { get; set; }

        [JsonPropertyName("num_times_rewatched")]
        public int NumTimesRewatched { get; set; }

        [JsonPropertyName("rewatch_value")]
        public int RewatchValue { get; set; }

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("comments")]
        public string Comments { get; set; } = string.Empty;
    }

    // DTOs for API responses
    public class CompletedAnimeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EnglishTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double? Score { get; set; }
        public int UserScore { get; set; }
        public int? Rank { get; set; }
        public int? Popularity { get; set; }
        public int? NumEpisodes { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public DateTime CompletedAt { get; set; }
        public string FinishDate { get; set; } = string.Empty;
    }

    public class CurrentlyWatchingAnimeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EnglishTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double? Score { get; set; }
        public int UserScore { get; set; }
        public int? Rank { get; set; }
        public int? Popularity { get; set; }
        public int? NumEpisodes { get; set; }
        public int NumEpisodesWatched { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Studios { get; set; } = new();
        public bool IsRewatching { get; set; }
        public DateTime LastUpdated { get; set; }
        public string StartDate { get; set; } = string.Empty;
        public double ProgressPercentage { get; set; }
    }

    public class AnimeHistoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EnglishTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double? Score { get; set; }
        public int UserScore { get; set; }
        public int? Rank { get; set; }
        public int? Popularity { get; set; }
        public int? NumEpisodes { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Studios { get; set; } = new();
        public DateTime CompletedAt { get; set; }
        public string FinishDate { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public int NumTimesRewatched { get; set; }
        public string Comments { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
    }

    public class PlanToWatchAnimeDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string EnglishTitle { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public double? Score { get; set; }
        public int? Rank { get; set; }
        public int? Popularity { get; set; }
        public int? NumEpisodes { get; set; }
        public string MediaType { get; set; } = string.Empty;
        public string Rating { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = new();
        public List<string> Studios { get; set; } = new();
        public string Synopsis { get; set; } = string.Empty;
        public string StartDate { get; set; } = string.Empty;
        public string EndDate { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime AddedToListAt { get; set; }
        public List<string> Tags { get; set; } = new();
        public string Comments { get; set; } = string.Empty;
    }

    public class PagedAnimeHistoryResponse
    {
        public List<AnimeHistoryDto> Data { get; set; } = new();
        public PaginationInfo Pagination { get; set; } = new();
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
    }
}