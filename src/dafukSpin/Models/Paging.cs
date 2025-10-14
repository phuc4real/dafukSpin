using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Paging(
    [property: JsonPropertyName("previous")] string? Previous,
    [property: JsonPropertyName("next")] string? Next
);