using dafukSpin.Models;
using dafukSpin.Extensions;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Xunit;

namespace dafukSpin.Tests.Integration;

public sealed class PaginationUrlRewriteIntegrationTests
{
    [Fact]
    public void PaginationExtension_ShouldRewriteUrls_WithHttpContext()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("localhost:7069");

        var originalPaging = new Paging(
            Previous: null,
            Next: "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100&limit=50&status=completed"
        );
        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry>(),
            Paging: originalPaging
        );

        var rewriteService = new dafukSpin.Services.PaginationUrlRewriteService();
        var currentEndpoint = "/api/anime/users/testuser/anime/completed";

        // Act
        var result = response.RewritePaginationUrls(rewriteService, httpContext, currentEndpoint);

        // Assert
        result.Paging.Next.Should().NotBeNull();
        result.Paging.Next.Should().StartWith("https://localhost:7069/api/anime/users/testuser/anime/completed");
        result.Paging.Next.Should().Contain("offset=100");
        result.Paging.Next.Should().Contain("limit=50");
        result.Paging.Next.Should().Contain("status=completed");
    }

    [Fact]
    public void PaginationRewrite_ShouldMaintainJsonSerializability()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost:5244");

        var animeEntry = new AnimeEntry(
            Node: new AnimeNode(5114, "Fullmetal Alchemist: Brotherhood", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            ListStatus: new ListStatus("completed", 10, 64, false, "2023-04-01T12:00:00Z", null, null, null, null, null, null, null)
        );

        var originalPaging = new Paging(
            Previous: null,
            Next: "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100"
        );

        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry> { animeEntry },
            Paging: originalPaging
        );

        var rewriteService = new dafukSpin.Services.PaginationUrlRewriteService();
        var currentEndpoint = "/api/anime/users/testuser/anime/completed";

        // Act
        var rewrittenResponse = response.RewritePaginationUrls(rewriteService, httpContext, currentEndpoint);

        // Serialize to JSON to verify it works
        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            WriteIndented = true
        };
        var json = JsonSerializer.Serialize(rewrittenResponse, jsonOptions);

        // Deserialize back to verify structure
        var deserializedResponse = JsonSerializer.Deserialize<MyAnimeListResponse<AnimeEntry>>(json, jsonOptions);

        // Assert
        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.Data.Should().HaveCount(1);
        deserializedResponse.Data.First().Node.Id.Should().Be(5114);
        deserializedResponse.Data.First().Node.Title.Should().Be("Fullmetal Alchemist: Brotherhood");
        deserializedResponse.Paging.Next.Should().StartWith("http://localhost:5244/api/anime/users/testuser/anime/completed");
        deserializedResponse.Paging.Next.Should().Contain("offset=100");
        deserializedResponse.Paging.Previous.Should().BeNull();
    }

    [Theory]
    [InlineData("https", "api.example.com", "https://api.example.com")]
    [InlineData("http", "localhost:5244", "http://localhost:5244")]
    [InlineData("https", "localhost:7069", "https://localhost:7069")]
    public void PaginationRewrite_ShouldHandleDifferentBaseUrls(string scheme, string host, string expectedBase)
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = scheme;
        httpContext.Request.Host = new HostString(host);

        var originalPaging = new Paging(
            Previous: "https://api.myanimelist.net/v2/anime/ranking?ranking_type=tv&offset=0",
            Next: "https://api.myanimelist.net/v2/anime/ranking?ranking_type=tv&offset=100"
        );

        var response = new MyAnimeListResponse<AnimeRankingEntry>(
            Data: new List<AnimeRankingEntry>(),
            Paging: originalPaging
        );

        var rewriteService = new dafukSpin.Services.PaginationUrlRewriteService();
        var currentEndpoint = "/api/anime/ranking";

        // Act
        var result = response.RewritePaginationUrls(rewriteService, httpContext, currentEndpoint);

        // Assert
        result.Paging.Previous.Should().StartWith($"{expectedBase}/api/anime/ranking");
        result.Paging.Previous.Should().Contain("ranking_type=tv");
        result.Paging.Previous.Should().Contain("offset=0");

        result.Paging.Next.Should().StartWith($"{expectedBase}/api/anime/ranking");
        result.Paging.Next.Should().Contain("ranking_type=tv");
        result.Paging.Next.Should().Contain("offset=100");
    }
}