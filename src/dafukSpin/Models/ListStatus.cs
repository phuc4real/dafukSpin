using System.Text.Json.Serialization;

namespace dafukSpin.Models;

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