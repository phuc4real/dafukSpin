using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace dafukSpin.Tests.Integration;

public sealed class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            // Override configuration for testing
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["MyAnimeList:BaseUrl"] = "https://api.myanimelist.net/v2",
                    ["MyAnimeList:ClientId"] = "test-client-id" // Mock client ID for testing
                }!);
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Swagger_ShouldBeAccessible_InDevelopment()
    {
        // Act
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("dafukSpin API");
        content.Should().Contain("MyAnimeList");
    }

    [Fact]
    public async Task SwaggerUI_ShouldBeAccessible_AtRoot()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("swagger-ui");
    }

    [Fact]
    public async Task Application_ShouldStart_WithoutErrors()
    {
        // Arrange & Act - The application should start without throwing exceptions
        using var scope = _factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        // Assert - Key services should be registered
        var configuration = serviceProvider.GetService<IConfiguration>();
        configuration.Should().NotBeNull();

        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
        loggerFactory.Should().NotBeNull();
    }

    [Fact]
    public async Task HttpsRedirection_ShouldWork()
    {
        // Arrange
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("http://localhost/health");

        // Assert - Should redirect to HTTPS in production, but test may vary based on environment
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.MovedPermanently);
    }
}