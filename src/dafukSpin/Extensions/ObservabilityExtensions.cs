using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Diagnostics;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry observability
/// </summary>
public static class ObservabilityExtensions
{
    /// <summary>
    /// The name of the application for telemetry
    /// </summary>
    public const string ServiceName = "dafukSpin";

    /// <summary>
    /// The version of the application for telemetry
    /// </summary>
    public const string ServiceVersion = "1.0.0";

    /// <summary>
    /// Activity source for manual instrumentation
    /// </summary>
    public static readonly ActivitySource ActivitySource = new(ServiceName);

    /// <summary>
    /// Configures OpenTelemetry tracing and metrics
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The hosting environment</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinObservability(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        // Register the activity source
        services.AddSingleton(ActivitySource);

        // Configure OpenTelemetry
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(ServiceName, ServiceVersion)
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector())
            .WithTracing(builder =>
            {
                builder
                    .AddSource(ServiceName)
                    .AddAspNetCoreInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequest = (activity, httpRequest) =>
                        {
                            activity.SetTag("http.request.body.size", httpRequest.ContentLength);
                            activity.SetTag("http.client_ip", httpRequest.HttpContext.Connection.RemoteIpAddress?.ToString());
                            activity.SetTag("http.user_agent", httpRequest.Headers.UserAgent.FirstOrDefault());
                        };
                        options.EnrichWithHttpResponse = (activity, httpResponse) =>
                        {
                            activity.SetTag("http.response.body.size", httpResponse.ContentLength);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.type", exception.GetType().Name);
                            activity.SetTag("exception.message", exception.Message);
                        };
                    })
                    .AddHttpClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithHttpRequestMessage = (activity, httpRequestMessage) =>
                        {
                            activity.SetTag("http.request.method", httpRequestMessage.Method.Method);
                            activity.SetTag("http.request.uri", httpRequestMessage.RequestUri?.ToString());
                        };
                        options.EnrichWithHttpResponseMessage = (activity, httpResponseMessage) =>
                        {
                            activity.SetTag("http.response.status_code", (int)httpResponseMessage.StatusCode);
                        };
                        options.EnrichWithException = (activity, exception) =>
                        {
                            activity.SetTag("exception.type", exception.GetType().Name);
                            activity.SetTag("exception.message", exception.Message);
                        };
                    });

                // Add console exporter for development
                if (environment.IsDevelopment())
                {
                    builder.AddConsoleExporter();
                }

                // Add OTLP exporter if configured
                var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:Otlp:Endpoint");
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            })
            .WithMetrics(builder =>
            {
                builder
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddProcessInstrumentation();

                // Add console exporter for development
                if (environment.IsDevelopment())
                {
                    builder.AddConsoleExporter();
                }

                // Add OTLP exporter if configured
                var otlpEndpoint = configuration.GetValue<string>("OpenTelemetry:Otlp:Endpoint");
                if (!string.IsNullOrEmpty(otlpEndpoint))
                {
                    builder.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                    });
                }
            });

        return services;
    }

    /// <summary>
    /// Creates a new activity for manual instrumentation
    /// </summary>
    /// <param name="name">The name of the activity</param>
    /// <param name="kind">The kind of activity</param>
    /// <returns>The created activity or null if not sampled</returns>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return ActivitySource.StartActivity(name, kind);
    }

    /// <summary>
    /// Adds enrichment tags to the current activity
    /// </summary>
    /// <param name="tags">Dictionary of tags to add</param>
    public static void EnrichCurrentActivity(Dictionary<string, object?> tags)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            foreach (var tag in tags)
            {
                activity.SetTag(tag.Key, tag.Value?.ToString());
            }
        }
    }

    /// <summary>
    /// Adds a single tag to the current activity
    /// </summary>
    /// <param name="key">The tag key</param>
    /// <param name="value">The tag value</param>
    public static void AddTagToCurrentActivity(string key, object? value)
    {
        Activity.Current?.SetTag(key, value?.ToString());
    }

    /// <summary>
    /// Records an exception in the current activity
    /// </summary>
    /// <param name="exception">The exception to record</param>
    public static void RecordException(Exception exception)
    {
        var activity = Activity.Current;
        if (activity != null)
        {
            activity.SetStatus(ActivityStatusCode.Error, exception.Message);
            activity.SetTag("exception.type", exception.GetType().Name);
            activity.SetTag("exception.message", exception.Message);
            activity.SetTag("exception.stacktrace", exception.StackTrace);
        }
    }
}