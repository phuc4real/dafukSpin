using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Studio(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name
);