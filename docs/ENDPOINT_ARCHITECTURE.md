# Endpoint Architecture

This project uses a modular endpoint architecture with automatic registration based on the `IEndpoint` interface pattern.

## Architecture Overview

### IEndpoint Interface
All endpoints implement the `IEndpoint` interface which requires a `MapEndpoint` method:

```csharp
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
```

### Automatic Registration
Endpoints are automatically discovered and registered using reflection in the `EndpointExtensions.MapEndpoints()` method.

## Current Endpoints

### System Endpoints
- **HealthEndpoints**: `/health` - System health check

### User Anime Lists
- **UserAnimeListEndpoints**: `/api/anime/users/{username}/anime/*`
  - `GET /anime` - Complete anime list
  - `GET /anime/completed` - Completed anime
  - `GET /anime/watching` - Currently watching
  - `GET /anime/plan-to-watch` - Plan to watch
  - `GET /anime/on-hold` - On hold
  - `GET /anime/dropped` - Dropped

### Anime Data
- **AnimeDataEndpoints**: `/api/anime/*`
  - `GET /details/{animeId}` - Anime details
  - `GET /search` - Search anime

### Rankings & Seasonal
- **AnimeRankingEndpoints**: `/api/anime/*`
  - `GET /ranking` - Anime rankings
  - `GET /seasonal/{year}/{season}` - Seasonal anime
  - `GET /suggested` - Suggested anime

## Adding New Endpoints

1. Create a new class implementing `IEndpoint`
2. Place it in the `Endpoints` folder
3. Implement the `MapEndpoint` method
4. The endpoint will be automatically registered on startup

Example:
```csharp
public sealed class MyNewEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/mynew")
            .WithTags("My New Feature");
            
        group.MapGet("/", GetDataAsync)
            .WithName("GetMyNewData")
            .WithSummary("Get my new data");
    }
    
    private static async Task<IResult> GetDataAsync()
    {
        return Results.Ok("Hello World");
    }
}
```

## Benefits

- **Modular**: Each endpoint group is self-contained
- **Automatic**: No manual registration required
- **Testable**: Each endpoint class can be tested independently
- **Maintainable**: Clear separation of concerns
- **Discoverable**: All endpoints are automatically found via reflection