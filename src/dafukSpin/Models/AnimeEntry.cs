using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AnimeEntry(
    [property: JsonPropertyName("node")] AnimeNode Node,
    [property: JsonPropertyName("list_status")] ListStatus? ListStatus
);