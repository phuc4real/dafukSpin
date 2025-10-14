using dafukSpin.Services;
using Refit;
using System.Text.Json;
using Polly;
using Polly.Extensions.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure JSON serialization options for MyAnimeList API compatibility
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.SerializerOptions.WriteIndented = true;
});

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
builder.Services.AddRefitClient<IMyAnimeListApi>(refitSettings)
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
builder.Services.AddScoped<IMyAnimeListService, MyAnimeListService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "dafukSpin API",
        Version = "v1",
        Description = "API for anime data via MyAnimeList Official API - Using Minimal APIs with Refit and Client ID Authentication",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "dafukSpin",
            Url = new Uri("https://github.com/phuc4real/dafukSpin")
        }
    });

    // Add XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add authentication requirement info to Swagger
    options.AddSecurityDefinition("MAL-ClientId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "X-MAL-CLIENT-ID",
        Description = "MyAnimeList API Client ID for authentication"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "dafukSpin API v1");
        options.RoutePrefix = string.Empty; // Makes Swagger UI available at root
        options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        options.DefaultModelsExpandDepth(-1); // Hide model schemas by default
    });
}

app.UseHttpsRedirection();

// Add a health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithSummary("Health check endpoint")
    .WithDescription("Returns the health status of the API")
    .Produces(200);

app.Run();

// Make Program accessible for integration testing
public partial class Program { }
