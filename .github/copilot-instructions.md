# dafukSpin - Copilot Instructions

## Project Overview
This is a .NET 8 ASP.NET Core Web API that integrates with the official MyAnimeList API to provide anime data services. The project uses **Minimal APIs** architecture instead of traditional controllers, with a clean separation between services, models, and endpoint mappings.

## Architecture Patterns

### Minimal API Structure
- **No Controllers**: The project has migrated from controller-based to minimal APIs
- **Endpoint Extensions**: API endpoints are defined in `Extensions/MyAnimeListEndpoints.cs` using `IEndpointRouteBuilder` extensions
- **Service Registration**: Services are registered in `Program.cs` with dependency injection
- **Route Groups**: Endpoints are organized using `MapGroup("/api/anime")` with shared tags and metadata

### Key Architectural Decisions
```csharp
// In Program.cs - Services are registered with HttpClient
builder.Services.AddHttpClient<IMyAnimeListService, MyAnimeListService>();

// Endpoints are mapped via extension method
app.MapMyAnimeListEndpoints();
```

### File Organization
```
src/dafukSpin/
├── Services/           # Business logic and external API integration
├── Models/            # DTOs and data models with JsonPropertyName attributes
├── Extensions/        # Minimal API endpoint mappings
└── Controllers/       # Empty (migrated to minimal APIs)
```

## Development Patterns

### API Integration Pattern
- **HttpClient Injection**: Services receive `HttpClient` via DI for external API calls
- **Authentication Required**: MyAnimeList API requires Client ID authentication
- **Base URL Configuration**: MyAnimeList API base URL configured in service constructors

### Model Conventions
- All external API models use `[JsonPropertyName("snake_case")]` attributes for JSON serialization
- DTOs follow pattern: `{Entity}{Action}Dto` (e.g., `CompletedAnimeDto`, `CurrentlyWatchingAnimeDto`)
- Response wrappers like `PagedAnimeHistoryResponse` for complex return types

### Error Handling Pattern
```csharp
// Services log and handle HTTP client exceptions
_logger.LogError(ex, "Error fetching data for user: {Username}", username);
return new List<CompletedAnimeDto>();
```

## Development Workflow

### Running the Application
- **Development URL**: `https://localhost:7069` (HTTPS) or `http://localhost:5244` (HTTP)
- **Swagger UI**: Available at root URL (`/`) in development - configured via `RoutePrefix = string.Empty`
- **Docker**: Multi-stage Dockerfile with .NET 8 runtime, exposes ports 8080/8081

### Configuration Requirements
- Configuration required! MyAnimeList API requires Client ID authentication

### Testing Endpoints
- Use the `.http` files in the project root for API testing
- Base URL variable: `@dafukSpin_HostAddress = http://localhost:5244`

## Service Implementation Guidelines

### External API Services
- Implement interface pattern: `IMyAnimeListService` → `MyAnimeListService`
- Configure base URLs and headers in constructor
- Use async/await with proper exception handling
- Return empty collections rather than null for list operations

### Endpoint Mapping
- Group related endpoints using `MapGroup()`
- Add comprehensive OpenAPI metadata with `WithSummary()`, `WithDescription()`
- Specify all possible HTTP status codes with `Produces<T>()`
- Use descriptive endpoint names with `WithName()`

### When Adding New Features
1. Create models in `Models/` with proper JSON serialization attributes
2. Implement service interface and concrete class in `Services/`
3. Register service with DI in `Program.cs`
4. Add endpoint mappings in new or existing `Extensions/{Feature}Endpoints.cs`
5. Map endpoints in `Program.cs` with `app.Map{Feature}Endpoints()`

## Coding Style & Guidelines

### C# Language Patterns
- **Prefer records for DTOs**: Use `public sealed record CustomerDto(string Name, string Email)` instead of classes with mutable properties
- **Make classes sealed by default**: Only leave unsealed when inheritance is specifically designed
- **Use var for type inference**: `var order = new Order()` when type is clear from context
- **File-scoped namespaces**: Use `namespace MyNamespace;` instead of block-scoped namespaces
- **Collection initializers**: Prefer `string[] fruits = ["Apple", "Banana"];` syntax

### Nullability & Safety
- **Explicit nullability**: Mark nullable fields with `?` and use `[NotNull]` attributes appropriately
- **Try methods over exceptions**: Use `int.TryParse()` instead of exception handling for expected cases
- **Null checks for public methods only**: Use `ArgumentNullException.ThrowIfNull()` for reference types in public APIs
- **Pattern matching for null checks**: Use switch expressions for cleaner null validation

### Asynchronous Programming
- **Always flow CancellationToken**: Include `CancellationToken cancellationToken = default` in async methods
- **Prefer ValueTask for pre-computed values**: Use `ValueTask<int>` for zero allocations when returning computed values
- **Never use Task.Result or Task.Wait**: Always use async/await pattern to avoid deadlocks
- **Dispose CancellationTokenSource**: Use `using var cts = new CancellationTokenSource()` pattern

### Code Organization
- **Separate state from behavior**: Use records for data, static classes for operations
- **Prefer pure methods**: Functions should not have side effects when possible
- **Minimal dependencies**: Avoid constructor injection of too many services
- **Use nameof operator**: Always use `nameof()` for property names in exceptions, logging, and attributes

### Symbol References
- **Implicit usings**: Rely on global usings instead of explicit using statements
- **nameof in logging**: Use `_logger.LogInformation("Processing {Method}", nameof(ProcessAsync))`
- **nameof in exceptions**: `throw new ArgumentNullException(nameof(order))`

## Project-Specific Notes
- The project name "dafukSpin" should be treated as a proper noun in code and comments
- Swagger is configured to display at the root URL for easy development access
- All anime-related endpoints follow RESTful patterns under `/api/anime/`
- The codebase prioritizes explicit configuration and clear separation of concerns over magic conventions