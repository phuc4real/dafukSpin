using dafukSpin.Models;
using dafukSpin.Services;
using FluentAssertions;
using Xunit;

namespace dafukSpin.Tests.Services;

public sealed class PaginationUrlRewriteServiceTests
{
    private readonly PaginationUrlRewriteService _service = new();
    private const string BaseUrl = "https://localhost:7069";
    private const string CurrentEndpoint = "/api/anime/users/testuser/anime/completed";

    [Fact]
    public void RewritePaginationUrls_ShouldRewriteNextUrl_WhenNextUrlExists()
    {
        // Arrange
        var originalPaging = new Paging(
            Previous: null,
            Next: "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100&limit=50&status=completed"
        );
        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry>(),
            Paging: originalPaging
        );

        // Act
        var result = _service.RewritePaginationUrls(response, BaseUrl, CurrentEndpoint);

        // Assert
        result.Paging.Next.Should().NotBeNull();
        result.Paging.Next.Should().StartWith($"{BaseUrl}{CurrentEndpoint}");
        result.Paging.Next.Should().Contain("offset=100");
        result.Paging.Next.Should().Contain("limit=50");
        result.Paging.Next.Should().Contain("status=completed");
        result.Paging.Previous.Should().BeNull();
    }

    [Fact]
    public void RewritePaginationUrls_ShouldRewritePreviousUrl_WhenPreviousUrlExists()
    {
        // Arrange
        var originalPaging = new Paging(
            Previous: "https://api.myanimelist.net/v2/users/testuser/animelist?offset=0&limit=50&status=completed",
            Next: null
        );
        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry>(),
            Paging: originalPaging
        );

        // Act
        var result = _service.RewritePaginationUrls(response, BaseUrl, CurrentEndpoint);

        // Assert
        result.Paging.Previous.Should().NotBeNull();
        result.Paging.Previous.Should().StartWith($"{BaseUrl}{CurrentEndpoint}");
        result.Paging.Previous.Should().Contain("offset=0");
        result.Paging.Previous.Should().Contain("limit=50");
        result.Paging.Previous.Should().Contain("status=completed");
        result.Paging.Next.Should().BeNull();
    }

    [Fact]
    public void RewritePaginationUrls_ShouldHandleNullUrls()
    {
        // Arrange
        var originalPaging = new Paging(Previous: null, Next: null);
        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry>(),
            Paging: originalPaging
        );

        // Act
        var result = _service.RewritePaginationUrls(response, BaseUrl, CurrentEndpoint);

        // Assert
        result.Paging.Previous.Should().BeNull();
        result.Paging.Next.Should().BeNull();
    }

    [Fact]
    public void ExtractQueryParameters_ShouldExtractAllParameters()
    {
        // Arrange
        var url = "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100&limit=50&status=completed&sort=list_score";

        // Act
        var result = _service.ExtractQueryParameters(url);

        // Assert
        result.Should().HaveCount(4);
        result["offset"].Should().Be("100");
        result["limit"].Should().Be("50");
        result["status"].Should().Be("completed");
        result["sort"].Should().Be("list_score");
    }

    [Fact]
    public void ExtractQueryParameters_ShouldHandleEmptyUrl()
    {
        // Arrange
        var url = string.Empty;

        // Act
        var result = _service.ExtractQueryParameters(url);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractQueryParameters_ShouldHandleNullUrl()
    {
        // Arrange
        string? url = null;

        // Act
        var result = _service.ExtractQueryParameters(url!);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ExtractQueryParameters_ShouldHandleUrlWithoutQuery()
    {
        // Arrange
        var url = "https://api.myanimelist.net/v2/users/testuser/animelist";

        // Act
        var result = _service.ExtractQueryParameters(url);

        // Assert
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("/api/anime/users/testuser/anime", "https://localhost:7069/api/anime/users/testuser/anime")]
    [InlineData("api/anime/search", "https://localhost:7069/api/anime/search")]
    [InlineData("/api/anime/ranking", "https://localhost:7069/api/anime/ranking")]
    public void RewritePaginationUrls_ShouldHandleDifferentEndpointFormats(string endpoint, string expectedBaseUrl)
    {
        // Arrange
        var originalPaging = new Paging(
            Previous: null,
            Next: "https://api.myanimelist.net/v2/anime?q=test&offset=100"
        );
        var response = new MyAnimeListResponse<AnimeSearchResult>(
            Data: new List<AnimeSearchResult>(),
            Paging: originalPaging
        );

        // Act
        var result = _service.RewritePaginationUrls(response, BaseUrl, endpoint);

        // Assert
        result.Paging.Next.Should().StartWith(expectedBaseUrl);
        result.Paging.Next.Should().Contain("q=test");
        result.Paging.Next.Should().Contain("offset=100");
    }

    [Fact]
    public void RewritePaginationUrls_ShouldPreserveOriginalData()
    {
        // Arrange
        var animeEntry = new AnimeEntry(
            Node: new AnimeNode(1, "Test Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            ListStatus: new ListStatus("completed", 9, 12, false, "2023-01-01T00:00:00Z", null, null, null, null, null, null, null)
        );
        var originalPaging = new Paging(
            Previous: null,
            Next: "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100"
        );
        var response = new MyAnimeListResponse<AnimeEntry>(
            Data: new List<AnimeEntry> { animeEntry },
            Paging: originalPaging
        );

        // Act
        var result = _service.RewritePaginationUrls(response, BaseUrl, CurrentEndpoint);

        // Assert
        result.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(1);
        result.Data.First().Node.Title.Should().Be("Test Anime");
        result.Data.First().ListStatus.Status.Should().Be("completed");
    }
}