namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring CORS policies
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Configures CORS policy for frontend applications
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="allowedOrigins">Optional array of allowed origins. Defaults to localhost:4200</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinCors(this IServiceCollection services, params string[] allowedOrigins)
    {
        // Default origins if none provided
        var origins = allowedOrigins.Length > 0
            ? allowedOrigins
            : new[] { "http://localhost:4200", "https://localhost:4200" };

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(origins)
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    /// Enables CORS middleware with the configured policy
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseDafukSpinCors(this WebApplication app)
    {
        app.UseCors("AllowFrontend");
        return app;
    }
}