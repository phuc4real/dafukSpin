using dafukSpin.Services;
using Refit;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring Refit HTTP clients
/// </summary>
public static class RefitExtensions
{
    /// <summary>
    /// Configures Refit client for MyAnimeList API with authentication and retry policies
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddMyAnimeListApi(this IServiceCollection services)
    {
        // Configure Refit for MyAnimeList API
        var refitSettings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                }
            )
        };

        // Add Refit client for MyAnimeList API with authentication
        services.AddRefitClient<IMyAnimeListApi>(refitSettings)
            .ConfigureHttpClient((serviceProvider, httpClient) =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

                // Configure base URL and authentication
                var baseUrl = configuration["MyAnimeList:BaseUrl"] ?? "https://api.myanimelist.net/v2";
                var clientId = configuration["MyAnimeList:ClientId"];
                var clientSecret = configuration["MyAnimeList:ClientSecret"]; // Available for future OAuth implementation

                if (string.IsNullOrEmpty(clientId))
                {
                    logger.LogError("MyAnimeList ClientId is not configured. Please set it using: dotnet user-secrets set \"MyAnimeList:ClientId\" \"your-client-id\"");
                    throw new InvalidOperationException("MyAnimeList ClientId not configured. Please configure it in user secrets.");
                }

                logger.LogInformation("Configuring MyAnimeList API client with base URL: {BaseUrl}", baseUrl);

                httpClient.BaseAddress = new Uri(baseUrl);
                httpClient.DefaultRequestHeaders.Add("X-MAL-CLIENT-ID", clientId);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "dafukSpin/1.0");

                // Set reasonable timeout
                httpClient.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddPolicyHandler(HttpPolicyExtensions
                .HandleTransientHttpError() // HttpRequestException and 5XX and 408 status codes
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        // Register the service wrapper
        services.AddScoped<IMyAnimeListService, MyAnimeListService>();

        // Register pagination URL rewrite service
        services.AddScoped<IPaginationUrlRewriteService, PaginationUrlRewriteService>();

        return services;
    }
}