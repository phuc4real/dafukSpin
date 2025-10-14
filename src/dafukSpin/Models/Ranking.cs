using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Ranking(
    [property: JsonPropertyName("rank")] int Rank,
    [property: JsonPropertyName("previous_rank")] int? PreviousRank
);