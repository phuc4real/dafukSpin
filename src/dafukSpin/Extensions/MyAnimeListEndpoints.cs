using Microsoft.AspNetCore.Mvc;
using dafukSpin.Models;
using dafukSpin.Services;

namespace dafukSpin.Extensions
{
    public static class MyAnimeListEndpoints
    {
        public static void MapMyAnimeListEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("/api/myanimelist")
                .WithTags("MyAnimeList");

            // Get Top 10 Latest Completed Anime
            group.MapGet("/users/{username}/completed-anime", GetTop10LatestCompletedAnime)
                .WithName("GetTop10LatestCompletedAnime")
                .WithSummary("Gets the top 10 latest completed anime for a specific user from MyAnimeList")
                .WithDescription("Returns a list of the top 10 latest completed anime for the specified MyAnimeList user")
                .Produces<List<CompletedAnimeDto>>(200)
                .Produces(400)
                .Produces(401)
                .Produces(404)
                .Produces(500)
                .Produces(502);

            // Get Top 5 Currently Watching Anime
            group.MapGet("/users/{username}/currently-watching", GetTop5CurrentlyWatchingAnime)
                .WithName("GetTop5CurrentlyWatchingAnime")
                .WithSummary("Gets the top 5 currently watching anime for a specific user from MyAnimeList")
                .WithDescription("Returns a list of the top 5 currently watching anime with progress information")
                .Produces<List<CurrentlyWatchingAnimeDto>>(200)
                .Produces(400)
                .Produces(401)
                .Produces(404)
                .Produces(500)
                .Produces(502);

            // Get User Anime History (Paginated)
            group.MapGet("/users/{username}/history", GetUserAnimeHistory)
                .WithName("GetUserAnimeHistory")
                .WithSummary("Gets the anime history (all statuses) for a specific user from MyAnimeList with pagination")
                .WithDescription("Returns paginated anime history including all statuses (completed, watching, on hold, dropped, plan to watch)")
                .Produces<PagedAnimeHistoryResponse>(200)
                .Produces(400)
                .Produces(401)
                .Produces(404)
                .Produces(500)
                .Produces(502);

            // Get Random Plan to Watch Anime
            group.MapGet("/users/{username}/random-plan-to-watch", GetRandomPlanToWatchAnime)
                .WithName("GetRandomPlanToWatchAnime")
                .WithSummary("Gets a random anime from the user's plan to watch list")
                .WithDescription("Returns a randomly selected anime from the user's plan to watch list, or 404 if none found")
                .Produces<PlanToWatchAnimeDto>(200)
                .Produces(400)
                .Produces(401)
                .Produces(404)
                .Produces(500)
                .Produces(502);

            // Health Check
            group.MapGet("/health", GetHealth)
                .WithName("GetMyAnimeListHealth")
                .WithSummary("Health check endpoint for the MyAnimeList service")
                .WithDescription("Returns the service status, timestamp, and available endpoints")
                .Produces(200);
        }

        private static async Task<IResult> GetTop10LatestCompletedAnime(
            string username,
            IMyAnimeListService myAnimeListService,
            ILogger<Program> logger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    logger.LogWarning("Empty or null username provided");
                    return Results.BadRequest("Username cannot be empty");
                }

                logger.LogInformation("Received request for completed anime of user: {Username}", username);

                var completedAnime = await myAnimeListService.GetTop10LatestCompletedAnimeAsync(username);

                if (!completedAnime.Any())
                {
                    logger.LogInformation("No completed anime found for user: {Username}", username);
                    return Results.Ok(new List<CompletedAnimeDto>());
                }

                logger.LogInformation("Successfully returned {Count} completed anime for user: {Username}",
                    completedAnime.Count, username);

                return Results.Ok(completedAnime);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument provided for username: {Username}", username);
                return Results.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Authentication error for MyAnimeList API");
                return Results.Unauthorized();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Client ID"))
            {
                logger.LogError(ex, "MyAnimeList Client ID configuration error");
                return Results.Problem("MyAnimeList API is not properly configured", statusCode: 500);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "External API error for user: {Username}", username);
                return Results.Problem("Failed to fetch data from MyAnimeList. Please try again later.", statusCode: 502);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Data processing error for user: {Username}", username);
                return Results.Problem("Failed to process MyAnimeList data", statusCode: 500);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred for user: {Username}", username);
                return Results.Problem("An unexpected error occurred", statusCode: 500);
            }
        }

        private static async Task<IResult> GetTop5CurrentlyWatchingAnime(
            string username,
            IMyAnimeListService myAnimeListService,
            ILogger<Program> logger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    logger.LogWarning("Empty or null username provided for currently watching");
                    return Results.BadRequest("Username cannot be empty");
                }

                logger.LogInformation("Received request for currently watching anime of user: {Username}", username);

                var currentlyWatchingAnime = await myAnimeListService.GetTop5CurrentlyWatchingAnimeAsync(username);

                if (!currentlyWatchingAnime.Any())
                {
                    logger.LogInformation("No currently watching anime found for user: {Username}", username);
                    return Results.Ok(new List<CurrentlyWatchingAnimeDto>());
                }

                logger.LogInformation("Successfully returned {Count} currently watching anime for user: {Username}",
                    currentlyWatchingAnime.Count, username);

                return Results.Ok(currentlyWatchingAnime);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument provided for username: {Username}", username);
                return Results.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Authentication error for MyAnimeList API");
                return Results.Unauthorized();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Client ID"))
            {
                logger.LogError(ex, "MyAnimeList Client ID configuration error");
                return Results.Problem("MyAnimeList API is not properly configured", statusCode: 500);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "External API error for user: {Username}", username);
                return Results.Problem("Failed to fetch data from MyAnimeList. Please try again later.", statusCode: 502);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Data processing error for user: {Username}", username);
                return Results.Problem("Failed to process MyAnimeList data", statusCode: 500);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred for user: {Username}", username);
                return Results.Problem("An unexpected error occurred", statusCode: 500);
            }
        }

        private static async Task<IResult> GetUserAnimeHistory(
            string username,
            [FromQuery] int page,
            [FromQuery] int pageSize,
            IMyAnimeListService myAnimeListService,
            ILogger<Program> logger)
        {
            try
            {
                // Set defaults if not provided
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 20;

                if (string.IsNullOrWhiteSpace(username))
                {
                    logger.LogWarning("Empty or null username provided for anime history");
                    return Results.BadRequest("Username cannot be empty");
                }

                if (page < 1)
                {
                    logger.LogWarning("Invalid page number provided: {Page}", page);
                    return Results.BadRequest("Page number must be greater than 0");
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    logger.LogWarning("Invalid page size provided: {PageSize}", pageSize);
                    return Results.BadRequest("Page size must be between 1 and 100");
                }

                logger.LogInformation("Received request for anime history of user: {Username}, page: {Page}, pageSize: {PageSize}",
                    username, page, pageSize);

                var animeHistory = await myAnimeListService.GetUserAnimeHistoryAsync(username, page, pageSize);

                logger.LogInformation("Successfully returned anime history for user: {Username}. Page {Page}/{TotalPages}, {Count} items",
                    username, animeHistory.Pagination.CurrentPage, animeHistory.Pagination.TotalPages, animeHistory.Data.Count);

                return Results.Ok(animeHistory);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument provided for username: {Username}", username);
                return Results.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Authentication error for MyAnimeList API");
                return Results.Unauthorized();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Client ID"))
            {
                logger.LogError(ex, "MyAnimeList Client ID configuration error");
                return Results.Problem("MyAnimeList API is not properly configured", statusCode: 500);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "External API error for user: {Username}", username);
                return Results.Problem("Failed to fetch data from MyAnimeList. Please try again later.", statusCode: 502);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Data processing error for user: {Username}", username);
                return Results.Problem("Failed to process MyAnimeList data", statusCode: 500);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred for user: {Username}", username);
                return Results.Problem("An unexpected error occurred", statusCode: 500);
            }
        }

        private static async Task<IResult> GetRandomPlanToWatchAnime(
            string username,
            IMyAnimeListService myAnimeListService,
            ILogger<Program> logger)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    logger.LogWarning("Empty or null username provided for random plan to watch");
                    return Results.BadRequest("Username cannot be empty");
                }

                logger.LogInformation("Received request for random plan to watch anime for user: {Username}", username);

                var randomAnime = await myAnimeListService.GetRandomPlanToWatchAnimeAsync(username);

                if (randomAnime == null)
                {
                    logger.LogInformation("No plan to watch anime found for user: {Username}", username);
                    return Results.NotFound("No anime found in the user's plan to watch list");
                }

                logger.LogInformation("Successfully returned random plan to watch anime '{Title}' for user: {Username}",
                    randomAnime.Title, username);

                return Results.Ok(randomAnime);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid argument provided for username: {Username}", username);
                return Results.NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Authentication error for MyAnimeList API");
                return Results.Unauthorized();
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Client ID"))
            {
                logger.LogError(ex, "MyAnimeList Client ID configuration error");
                return Results.Problem("MyAnimeList API is not properly configured", statusCode: 500);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "External API error for user: {Username}", username);
                return Results.Problem("Failed to fetch data from MyAnimeList. Please try again later.", statusCode: 502);
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError(ex, "Data processing error for user: {Username}", username);
                return Results.Problem("Failed to process MyAnimeList data", statusCode: 500);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error occurred for user: {Username}", username);
                return Results.Problem("An unexpected error occurred", statusCode: 500);
            }
        }

        private static IResult GetHealth()
        {
            return Results.Ok(new
            {
                Status = "Healthy",
                Service = "MyAnimeList API Integration",
                Timestamp = DateTime.UtcNow,
                AvailableEndpoints = new[]
                {
                    "GET /api/myanimelist/users/{username}/completed-anime",
                    "GET /api/myanimelist/users/{username}/currently-watching",
                    "GET /api/myanimelist/users/{username}/history?page={page}&pageSize={pageSize}",
                    "GET /api/myanimelist/users/{username}/random-plan-to-watch"
                }
            });
        }
    }
}