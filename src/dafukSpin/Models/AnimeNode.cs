using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AnimeNode(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("main_picture")] Picture? MainPicture,
    [property: JsonPropertyName("alternative_titles")] AlternativeTitles? AlternativeTitles,
    [property: JsonPropertyName("start_date")] string? StartDate,
    [property: JsonPropertyName("end_date")] string? EndDate,
    [property: JsonPropertyName("synopsis")] string? Synopsis,
    [property: JsonPropertyName("mean")] double? Mean,
    [property: JsonPropertyName("rank")] int? Rank,
    [property: JsonPropertyName("popularity")] int? Popularity,
    [property: JsonPropertyName("num_episodes")] int? NumEpisodes,
    [property: JsonPropertyName("start_season")] StartSeason? StartSeason,
    [property: JsonPropertyName("broadcast")] Broadcast? Broadcast,
    [property: JsonPropertyName("source")] string? Source,
    [property: JsonPropertyName("average_episode_duration")] int? AverageEpisodeDuration,
    [property: JsonPropertyName("rating")] string? Rating,
    [property: JsonPropertyName("pictures")] IReadOnlyList<Picture>? Pictures,
    [property: JsonPropertyName("background")] string? Background,
    [property: JsonPropertyName("genres")] IReadOnlyList<Genre>? Genres,
    [property: JsonPropertyName("studios")] IReadOnlyList<Studio>? Studios,
    [property: JsonPropertyName("media_type")] string? MediaType,
    [property: JsonPropertyName("status")] string? Status,
    [property: JsonPropertyName("my_list_status")] ListStatus? MyListStatus,
    [property: JsonPropertyName("num_list_users")] int? NumListUsers,
    [property: JsonPropertyName("num_scoring_users")] int? NumScoringUsers,
    [property: JsonPropertyName("nsfw")] string? Nsfw,
    [property: JsonPropertyName("created_at")] string? CreatedAt,
    [property: JsonPropertyName("updated_at")] string? UpdatedAt
);