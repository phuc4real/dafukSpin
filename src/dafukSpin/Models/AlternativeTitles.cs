using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record AlternativeTitles(
    [property: JsonPropertyName("synonyms")] IReadOnlyList<string>? Synonyms,
    [property: JsonPropertyName("en")] string? En,
    [property: JsonPropertyName("ja")] string? Ja
);