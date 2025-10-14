using System.Text.Json.Serialization;

namespace dafukSpin.Models;

public sealed record Broadcast(
    [property: JsonPropertyName("day_of_the_week")] string? DayOfTheWeek,
    [property: JsonPropertyName("start_time")] string? StartTime
);