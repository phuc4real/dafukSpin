using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record MyAnimeListResponse<T>(
    [property: JsonPropertyName("data")] IReadOnlyList<T> Data,
    [property: JsonPropertyName("paging")] Paging Paging
);