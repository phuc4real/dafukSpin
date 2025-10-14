using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AnimeRankingEntry(
    [property: JsonPropertyName("node")] AnimeNode Node,
    [property: JsonPropertyName("ranking")] Ranking? Ranking
);