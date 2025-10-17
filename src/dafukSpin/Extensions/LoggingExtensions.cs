using Serilog;
using Serilog.Enrichers.CorrelationId;
using Serilog.Events;
using CorrelationId.DependencyInjection;
using CorrelationId;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring Serilog logging
/// </summary>
public static class LoggingExtensions
{
    /// <summary>
    /// Configures Serilog with correlation ID enrichment and structured logging
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinLogging(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Add correlation ID context accessor
        services.AddHttpContextAccessor();
        services.AddDefaultCorrelationId(options =>
        {
            options.AddToLoggingScope = true;
            options.EnforceHeader = false;
            options.IgnoreRequestHeader = false;
            options.IncludeInResponse = true;
            options.RequestHeader = "X-Correlation-ID";
            options.ResponseHeader = "X-Correlation-ID";
            options.UpdateTraceIdentifier = true;
        });

        // Configure Serilog
        Log.Logger = CreateSerilogLogger(configuration, environment);

        // Add Serilog to the services
        services.AddSerilog();

        return services;
    }

    /// <summary>
    /// Creates a Serilog logger configuration
    /// </summary>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <returns>The configured logger</returns>
    private static Serilog.ILogger CreateSerilogLogger(IConfiguration configuration, IWebHostEnvironment environment)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithCorrelationId()
            .Enrich.WithProperty("Application", "dafukSpin")
            .Enrich.WithProperty("Environment", environment.EnvironmentName)
            .Enrich.WithMachineName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId();

        if (environment.IsDevelopment())
        {
            loggerConfig
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{CorrelationId}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }
        else
        {
            loggerConfig
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] [{CorrelationId}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }

        return loggerConfig.CreateLogger();
    }

    /// <summary>
    /// Configures the web application to use correlation ID middleware
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseDafukSpinLogging(this WebApplication app)
    {
        // Use correlation ID middleware
        app.UseCorrelationId();

        // Use Serilog request logging
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.GetLevel = (httpContext, elapsed, ex) => ex != null
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode > 499
                    ? LogEventLevel.Error
                    : LogEventLevel.Information;

            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown");
                diagnosticContext.Set("ClientIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

                if (httpContext.User.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(httpContext.User.Identity.Name))
                {
                    diagnosticContext.Set("UserId", httpContext.User.Identity.Name);
                }
            };
        });

        return app;
    }
}