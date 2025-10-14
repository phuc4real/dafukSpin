using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AnimeSearchResult(
    [property: JsonPropertyName("node")] AnimeNode Node
);