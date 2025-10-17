namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring the complete dafukSpin application
/// </summary>
public static class ApplicationExtensions
{
    /// <summary>
    /// Configures all dafukSpin services in one call
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <param name="allowedOrigins">Optional CORS origins</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        params string[] allowedOrigins)
    {
        services.AddDafukSpinLogging(configuration, environment);
        services.AddDafukSpinObservability(configuration, environment);
        services.AddDafukSpinJson();
        services.AddDafukSpinCaching(configuration);
        services.AddMyAnimeListApi();
        services.AddDafukSpinCors(allowedOrigins);
        services.AddDafukSpinSwagger();

        return services;
    }

    /// <summary>
    /// Configures all dafukSpin middleware in the correct order
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseDafukSpinMiddleware(this WebApplication app)
    {
        app.UseDafukSpinLogging();
        app.UseHttpsRedirection();
        app.UseDafukSpinCors();
        app.UseDafukSpinSwagger();

        return app;
    }
}