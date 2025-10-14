using dafukSpin.Models;
using dafukSpin.Services;
using Microsoft.Extensions.Logging;
using Refit;

namespace dafukSpin.Tests.Services;

public sealed class MyAnimeListServiceTests
{
    private readonly Mock<IMyAnimeListApi> _mockApi;
    private readonly Mock<ILogger<MyAnimeListService>> _mockLogger;
    private readonly MyAnimeListService _service;

    public MyAnimeListServiceTests()
    {
        _mockApi = new Mock<IMyAnimeListApi>();
        _mockLogger = new Mock<ILogger<MyAnimeListService>>();
        _service = new MyAnimeListService(_mockApi.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetUserAnimeListAsync_ShouldReturnResults_WhenApiCallSucceeds()
    {
        // Arrange
        var username = "testuser";
        var animeEntry = new AnimeEntry(
            new AnimeNode(1, "Test Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            new ListStatus("watching", 8, 5, false, "2023-01-01T00:00:00Z", null, null, null, null, null, null, null)
        );
        var expectedResponse = new MyAnimeListResponse<AnimeEntry>(
            [animeEntry],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            It.Is<string>(s => s == username),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetUserAnimeListAsync(username);

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(1);
        result.Data.First().Node.Title.Should().Be("Test Anime");
    }

    [Fact]
    public async Task GetUserAnimeListAsync_ShouldReturnNull_WhenApiThrowsException()
    {
        // Arrange
        var username = "testuser";
        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var result = await _service.GetUserAnimeListAsync(username);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserAnimeListAsync_ShouldReturnNull_WhenApiThrowsApiException()
    {
        // Arrange
        var username = "testuser";
        var apiException = await ApiException.Create(
            new HttpRequestMessage(),
            HttpMethod.Get,
            new HttpResponseMessage(System.Net.HttpStatusCode.NotFound),
            new RefitSettings());

        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ThrowsAsync(apiException);

        // Act
        var result = await _service.GetUserAnimeListAsync(username);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetUserCompletedAnimeAsync_ShouldCallApiWithCompletedStatus()
    {
        // Arrange
        var username = "testuser";
        var expectedResponse = new MyAnimeListResponse<AnimeEntry>(
            [],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            username,
            "completed",
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetUserCompletedAnimeAsync(username);

        // Assert
        result.Should().NotBeNull();
        _mockApi.Verify(x => x.GetUserAnimeListAsync(
            username,
            "completed",
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetUserCurrentlyWatchingAnimeAsync_ShouldCallApiWithWatchingStatus()
    {
        // Arrange
        var username = "testuser";
        var expectedResponse = new MyAnimeListResponse<AnimeEntry>(
            [],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            username,
            "watching",
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetUserCurrentlyWatchingAnimeAsync(username);

        // Assert
        result.Should().NotBeNull();
        _mockApi.Verify(x => x.GetUserAnimeListAsync(
            username,
            "watching",
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAnimeDetailsAsync_ShouldReturnAnimeNode_WhenApiCallSucceeds()
    {
        // Arrange
        var animeId = 123;
        var expectedAnime = new AnimeNode(
            animeId,
            "Detailed Anime",
            new Picture("medium.jpg", "large.jpg"),
            null, null, null, "Great anime", 9.0, 10, 50, 24, null, null, "manga", 1440, "pg_13", null, null, null, null, null, null, null, null, null, null, null, null
        );

        _mockApi.Setup(x => x.GetAnimeDetailsAsync(
            animeId,
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedAnime);

        // Act
        var result = await _service.GetAnimeDetailsAsync(animeId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(animeId);
        result.Title.Should().Be("Detailed Anime");
        result.Mean.Should().Be(9.0);
    }

    [Fact]
    public async Task SearchAnimeAsync_ShouldReturnSearchResults_WhenApiCallSucceeds()
    {
        // Arrange
        var query = "test anime";
        var animeNode = new AnimeNode(456, "Search Result", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var searchResult = new AnimeSearchResult(animeNode);
        var expectedResponse = new MyAnimeListResponse<AnimeSearchResult>(
            [searchResult],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.SearchAnimeAsync(
            query,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.SearchAnimeAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(456);
        result.Data.First().Node.Title.Should().Be("Search Result");
    }

    [Fact]
    public async Task GetAnimeRankingAsync_ShouldReturnRankingResults_WhenApiCallSucceeds()
    {
        // Arrange
        var rankingType = "tv";
        var animeNode = new AnimeNode(789, "Top Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var ranking = new Ranking(1, null);
        var rankingEntry = new AnimeRankingEntry(animeNode, ranking);
        var expectedResponse = new MyAnimeListResponse<AnimeRankingEntry>(
            [rankingEntry],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetAnimeRankingAsync(
            rankingType,
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetAnimeRankingAsync(rankingType);

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(789);
        result.Data.First().Ranking!.Rank.Should().Be(1);
    }

    [Fact]
    public async Task GetSeasonalAnimeAsync_ShouldReturnSeasonalResults_WhenApiCallSucceeds()
    {
        // Arrange
        var year = 2023;
        var season = "winter";
        var animeNode = new AnimeNode(999, "Seasonal Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var seasonEntry = new AnimeSeasonEntry(animeNode);
        var expectedResponse = new MyAnimeListResponse<AnimeSeasonEntry>(
            [seasonEntry],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetSeasonalAnimeAsync(
            year,
            season,
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSeasonalAnimeAsync(year, season);

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(999);
        result.Data.First().Node.Title.Should().Be("Seasonal Anime");
    }

    [Theory]
    [InlineData("completed")]
    [InlineData("watching")]
    [InlineData("plan_to_watch")]
    [InlineData("on_hold")]
    [InlineData("dropped")]
    public async Task StatusSpecificMethods_ShouldPassCorrectStatus(string status)
    {
        // Arrange
        var username = "testuser";
        var expectedResponse = new MyAnimeListResponse<AnimeEntry>([], new Paging(null, null));

        _mockApi.Setup(x => x.GetUserAnimeListAsync(
            username,
            status,
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act & Assert
        var result = status switch
        {
            "completed" => await _service.GetUserCompletedAnimeAsync(username),
            "watching" => await _service.GetUserCurrentlyWatchingAnimeAsync(username),
            "plan_to_watch" => await _service.GetUserPlanToWatchAnimeAsync(username),
            "on_hold" => await _service.GetUserOnHoldAnimeAsync(username),
            "dropped" => await _service.GetUserDroppedAnimeAsync(username),
            _ => throw new ArgumentException("Invalid status")
        };

        result.Should().NotBeNull();
        _mockApi.Verify(x => x.GetUserAnimeListAsync(
            username,
            status,
            It.IsAny<string?>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSuggestedAnimeAsync_ShouldReturnSuggestions_WhenApiCallSucceeds()
    {
        // Arrange
        var animeEntry = new AnimeEntry(
            new AnimeNode(111, "Suggested Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            null
        );
        var expectedResponse = new MyAnimeListResponse<AnimeEntry>(
            [animeEntry],
            new Paging(null, null)
        );

        _mockApi.Setup(x => x.GetSuggestedAnimeAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<string?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _service.GetSuggestedAnimeAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Data.Should().HaveCount(1);
        result.Data.First().Node.Id.Should().Be(111);
        result.Data.First().Node.Title.Should().Be("Suggested Anime");
    }
}