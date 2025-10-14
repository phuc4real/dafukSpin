using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AnimeSeasonEntry(
    [property: JsonPropertyName("node")] AnimeNode Node
);