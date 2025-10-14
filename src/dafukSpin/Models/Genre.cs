using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Genre(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);