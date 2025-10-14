using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Picture(
    [property: JsonPropertyName("medium")] string? Medium,
    [property: JsonPropertyName("large")] string? Large
);