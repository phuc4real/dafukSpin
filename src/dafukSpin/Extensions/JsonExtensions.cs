using System.Text.Json;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring JSON serialization
/// </summary>
public static class JsonExtensions
{
    /// <summary>
    /// Configures JSON serialization options for MyAnimeList API compatibility
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinJson(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
            options.SerializerOptions.WriteIndented = true;
        });

        return services;
    }
}