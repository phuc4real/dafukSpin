using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record StartSeason(
    [property: JsonPropertyName("year")] int Year,
    [property: JsonPropertyName("season")] string Season
);