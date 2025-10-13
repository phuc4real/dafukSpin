using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record MyAnimeListResponse<T>(
    [property: JsonPropertyName("data")] IReadOnlyList<T> Data,
    [property: JsonPropertyName("paging")] Paging Paging
);

public sealed record Paging(
    [property: JsonPropertyName("previous")] string? Previous,
    [property: JsonPropertyName("next")] string? Next
);

public sealed record AnimeEntry(
    [property: JsonPropertyName("node")] AnimeNode Node,
    [property: JsonPropertyName("list_status")] ListStatus? ListStatus
);

public sealed record AnimeSearchResult(
    [property: JsonPropertyName("node")] AnimeNode Node
);

public sealed record AnimeRankingEntry(
    [property: JsonPropertyName("node")] AnimeNode Node,
    [property: JsonPropertyName("ranking")] Ranking? Ranking
);

public sealed record AnimeSeasonEntry(
    [property: JsonPropertyName("node")] AnimeNode Node
);

public sealed record Ranking(
    [property: JsonPropertyName("rank")] int Rank,
    [property: JsonPropertyName("previous_rank")] int? PreviousRank
);

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

public sealed record Picture(
    [property: JsonPropertyName("medium")] string? Medium,
    [property: JsonPropertyName("large")] string? Large
);

public sealed record AlternativeTitles(
    [property: JsonPropertyName("synonyms")] IReadOnlyList<string>? Synonyms,
    [property: JsonPropertyName("en")] string? En,
    [property: JsonPropertyName("ja")] string? Ja
);

public sealed record StartSeason(
    [property: JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("season")] string Season
);

public sealed record Broadcast(
    [property: JsonPropertyName("day_of_the_week")] string? DayOfTheWeek,
    [property: JsonPropertyName("start_time")] string? StartTime
);

public sealed record Genre(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);

public sealed record Studio(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);

public sealed record ListStatus(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("num_episodes_watched")] int? NumEpisodesWatched,
    [property: JsonPropertyName("is_rewatching")] bool? IsRewatching,
    [property: JsonPropertyName("updated_at")] string? UpdatedAt,
    [property: JsonPropertyName("start_date")] string? StartDate,
    [property: JsonPropertyName("finish_date")] string? FinishDate,
    [property: JsonPropertyName("priority")] int? Priority,
    [property: JsonPropertyName("num_times_rewatched")] int? NumTimesRewatched,
    [property: JsonPropertyName("rewatch_value")] int? RewatchValue,
    [property: JsonPropertyName("tags")] IReadOnlyList<string>? Tags,
    [property: JsonPropertyName("comments")] string? Comments
);

