using System.Text.Json;
using dafukSpin.Models;

namespace dafukSpin.Tests.Models;

public sealed class MyAnimeListModelsTests
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    [Fact]
    public void AnimeNode_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var animeNode = new AnimeNode(
            Id: 1,
            Title: "Test Anime",
            MainPicture: new Picture("medium.jpg", "large.jpg"),
            AlternativeTitles: new AlternativeTitles(["Synonym1", "Synonym2"], "English Title", "Japanese Title"),
            StartDate: "2023-01-01",
            EndDate: "2023-12-31",
            Synopsis: "Test synopsis",
            Mean: 8.5,
            Rank: 100,
            Popularity: 500,
            NumEpisodes: 24,
            StartSeason: new StartSeason(2023, "winter"),
            Broadcast: new Broadcast("monday", "21:00"),
            Source: "manga",
            AverageEpisodeDuration: 1440,
            Rating: "pg_13",
            Pictures: [new Picture("pic1.jpg", "pic1_large.jpg")],
            Background: "Test background",
            Genres: [new Genre(1, "Action"), new Genre(2, "Drama")],
            Studios: [new Studio(1, "Test Studio")],
            MediaType: "tv",
            Status: "finished_airing",
            MyListStatus: new ListStatus("completed", 9, 24, false, "2023-12-31T00:00:00Z", "2023-01-01", "2023-12-31", 0, 0, 0, [], "Great anime"),
            NumListUsers: 1000,
            NumScoringUsers: 800,
            Nsfw: "white",
            CreatedAt: "2023-01-01T00:00:00Z",
            UpdatedAt: "2023-12-31T00:00:00Z"
        );

        // Act
        var json = JsonSerializer.Serialize(animeNode, _jsonOptions);
        var deserializedAnimeNode = JsonSerializer.Deserialize<AnimeNode>(json, _jsonOptions);

        // Assert
        deserializedAnimeNode.Should().NotBeNull();
        deserializedAnimeNode!.Id.Should().Be(animeNode.Id);
        deserializedAnimeNode.Title.Should().Be(animeNode.Title);
        deserializedAnimeNode.MainPicture.Should().BeEquivalentTo(animeNode.MainPicture);
        deserializedAnimeNode.AlternativeTitles.Should().BeEquivalentTo(animeNode.AlternativeTitles);
        deserializedAnimeNode.Mean.Should().Be(animeNode.Mean);
        deserializedAnimeNode.Rank.Should().Be(animeNode.Rank);
        deserializedAnimeNode.NumEpisodes.Should().Be(animeNode.NumEpisodes);
        deserializedAnimeNode.StartSeason.Should().BeEquivalentTo(animeNode.StartSeason);
        deserializedAnimeNode.Broadcast.Should().BeEquivalentTo(animeNode.Broadcast);
        deserializedAnimeNode.Genres.Should().BeEquivalentTo(animeNode.Genres);
        deserializedAnimeNode.Studios.Should().BeEquivalentTo(animeNode.Studios);
        deserializedAnimeNode.MyListStatus.Should().BeEquivalentTo(animeNode.MyListStatus);
    }

    [Fact]
    public void MyAnimeListResponse_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var animeEntry = new AnimeEntry(
            new AnimeNode(1, "Test", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null),
            new ListStatus("watching", 8, 5, false, "2023-01-01T00:00:00Z", null, null, null, null, null, null, null)
        );

        var paging = new Paging("prev_url", "next_url");
        var response = new MyAnimeListResponse<AnimeEntry>([animeEntry], paging);

        // Act
        var json = JsonSerializer.Serialize(response, _jsonOptions);
        var deserializedResponse = JsonSerializer.Deserialize<MyAnimeListResponse<AnimeEntry>>(json, _jsonOptions);

        // Assert
        deserializedResponse.Should().NotBeNull();
        deserializedResponse!.Data.Should().HaveCount(1);
        deserializedResponse.Data.First().Node.Id.Should().Be(1);
        deserializedResponse.Data.First().Node.Title.Should().Be("Test");
        deserializedResponse.Paging.Previous.Should().Be("prev_url");
        deserializedResponse.Paging.Next.Should().Be("next_url");
    }

    [Fact]
    public void Picture_ShouldHandleNullValues()
    {
        // Arrange
        var picture = new Picture(null, null);

        // Act
        var json = JsonSerializer.Serialize(picture, _jsonOptions);
        var deserializedPicture = JsonSerializer.Deserialize<Picture>(json, _jsonOptions);

        // Assert
        deserializedPicture.Should().NotBeNull();
        deserializedPicture!.Medium.Should().BeNull();
        deserializedPicture.Large.Should().BeNull();
    }

    [Fact]
    public void ListStatus_ShouldSerializeAllFields()
    {
        // Arrange
        var listStatus = new ListStatus(
            Status: "completed",
            Score: 10,
            NumEpisodesWatched: 24,
            IsRewatching: true,
            UpdatedAt: "2023-12-31T00:00:00Z",
            StartDate: "2023-01-01",
            FinishDate: "2023-12-31",
            Priority: 1,
            NumTimesRewatched: 2,
            RewatchValue: 5,
            Tags: ["favorite", "masterpiece"],
            Comments: "Amazing anime!"
        );

        // Act
        var json = JsonSerializer.Serialize(listStatus, _jsonOptions);
        var deserializedStatus = JsonSerializer.Deserialize<ListStatus>(json, _jsonOptions);

        // Assert
        deserializedStatus.Should().NotBeNull();
        deserializedStatus!.Status.Should().Be("completed");
        deserializedStatus.Score.Should().Be(10);
        deserializedStatus.NumEpisodesWatched.Should().Be(24);
        deserializedStatus.IsRewatching.Should().BeTrue();
        deserializedStatus.Tags.Should().BeEquivalentTo(["favorite", "masterpiece"]);
        deserializedStatus.Comments.Should().Be("Amazing anime!");
    }

    [Fact]
    public void AlternativeTitles_ShouldHandleEmptyCollections()
    {
        // Arrange
        var titles = new AlternativeTitles([], null, null);

        // Act
        var json = JsonSerializer.Serialize(titles, _jsonOptions);
        var deserializedTitles = JsonSerializer.Deserialize<AlternativeTitles>(json, _jsonOptions);

        // Assert
        deserializedTitles.Should().NotBeNull();
        deserializedTitles!.Synonyms.Should().BeEmpty();
        deserializedTitles.En.Should().BeNull();
        deserializedTitles.Ja.Should().BeNull();
    }

    [Theory]
    [InlineData(2023, "winter")]
    [InlineData(2024, "spring")]
    [InlineData(2025, "summer")]
    [InlineData(2026, "fall")]
    public void StartSeason_ShouldHandleDifferentSeasons(int year, string season)
    {
        // Arrange
        var startSeason = new StartSeason(year, season);

        // Act
        var json = JsonSerializer.Serialize(startSeason, _jsonOptions);
        var deserializedSeason = JsonSerializer.Deserialize<StartSeason>(json, _jsonOptions);

        // Assert
        deserializedSeason.Should().NotBeNull();
        deserializedSeason!.Year.Should().Be(year);
        deserializedSeason.Season.Should().Be(season);
    }

    [Fact]
    public void Ranking_ShouldHandleNullPreviousRank()
    {
        // Arrange
        var ranking = new Ranking(1, null);

        // Act
        var json = JsonSerializer.Serialize(ranking, _jsonOptions);
        var deserializedRanking = JsonSerializer.Deserialize<Ranking>(json, _jsonOptions);

        // Assert
        deserializedRanking.Should().NotBeNull();
        deserializedRanking!.Rank.Should().Be(1);
        deserializedRanking.PreviousRank.Should().BeNull();
    }

    [Fact]
    public void AnimeSearchResult_ShouldContainAnimeNode()
    {
        // Arrange
        var animeNode = new AnimeNode(123, "Search Result", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var searchResult = new AnimeSearchResult(animeNode);

        // Act
        var json = JsonSerializer.Serialize(searchResult, _jsonOptions);
        var deserializedResult = JsonSerializer.Deserialize<AnimeSearchResult>(json, _jsonOptions);

        // Assert
        deserializedResult.Should().NotBeNull();
        deserializedResult!.Node.Id.Should().Be(123);
        deserializedResult.Node.Title.Should().Be("Search Result");
    }

    [Fact]
    public void AnimeRankingEntry_ShouldIncludeRankingInfo()
    {
        // Arrange
        var animeNode = new AnimeNode(456, "Ranked Anime", null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
        var ranking = new Ranking(10, 15);
        var rankingEntry = new AnimeRankingEntry(animeNode, ranking);

        // Act
        var json = JsonSerializer.Serialize(rankingEntry, _jsonOptions);
        var deserializedEntry = JsonSerializer.Deserialize<AnimeRankingEntry>(json, _jsonOptions);

        // Assert
        deserializedEntry.Should().NotBeNull();
        deserializedEntry!.Node.Id.Should().Be(456);
        deserializedEntry.Ranking!.Rank.Should().Be(10);
        deserializedEntry.Ranking.PreviousRank.Should().Be(15);
    }
}