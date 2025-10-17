# dafukSpin

A modern .NET 8 Web API that integrates with the official MyAnimeList API to provide anime data services. Built with minimal APIs, enterprise-grade observability, caching, and comprehensive error handling.

## Features

- **Official MyAnimeList API Integration**: Uses the official v2 API with proper authentication
- **Minimal APIs Architecture**: Modern ASP.NET Core endpoint mapping with clean organization
- **Comprehensive Anime Data**: User lists, anime details, search, rankings, and seasonal data
- **Refit HTTP Client**: Type-safe API client with automatic JSON serialization
- **Polly Resilience**: Built-in retry policies and circuit breakers
- **Enterprise Observability**: Structured logging with Serilog and OpenTelemetry tracing
- **Multi-Tier Caching**: Redis with memory cache fallback for optimal performance
- **Correlation ID Tracking**: Request tracing across all services and dependencies
- **Centralized Package Management**: Directory.Build.props and Directory.Packages.props for consistent versioning
- **Swagger Documentation**: Interactive API documentation at root URL
- **User Secrets**: Secure credential storage for development

## Architecture Highlights

- **Modern C# Patterns**: Records, pattern matching, file-scoped namespaces
- **Clean Separation**: Services, models, and endpoint mappings in separate layers
- **Type Safety**: Strongly-typed models with proper JSON serialization
- **Error Handling**: Comprehensive logging and graceful error responses
- **Observability First**: Full request tracing, metrics, and structured logging
- **Performance Optimized**: Multi-tier caching with Redis and memory fallback

### Design Patterns
- **Service Layer**: Clean abstraction over external API calls
- **Repository Pattern**: Structured data access with interfaces
- **Options Pattern**: Configuration binding with validation
- **Dependency Injection**: Clean service registration and lifecycle management
- **Record Types**: Immutable data structures with modern C# syntax
- **Sealed Classes**: Performance optimization and clear inheritance intent
- **File-scoped Namespaces**: Modern C# namespace declarations

## Getting Started

### Prerequisites
- .NET 8 SDK
- MyAnimeList API credentials (Client ID)
- Redis server (optional - falls back to memory cache)
- Internet connection (for MyAnimeList API access)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/phuc4real/dafukSpin.git
   cd dafukSpin
   ```

2. **Configure MyAnimeList API Credentials**
   
   The application requires MyAnimeList API credentials stored securely in user secrets:
   
   ```bash
   cd src/dafukSpin
   
   # Set your MyAnimeList Client ID (required)
   dotnet user-secrets set "MyAnimeList:ClientId" "your-client-id-here"
   
   # Set your MyAnimeList Client Secret (for future OAuth features)
   dotnet user-secrets set "MyAnimeList:ClientSecret" "your-client-secret-here"
   ```
   
   **Getting MyAnimeList API Credentials:**
   1. Visit [MyAnimeList API Configuration](https://myanimelist.net/apiconfig)
   2. Create a new API client application
   3. Copy the generated Client ID and Client Secret
   
   **Important:** Never commit API credentials to source control. They are securely stored in user secrets.

3. **Configure Redis (Optional)**
   
   For optimal performance, configure Redis caching:
   
   ```bash
   # Set Redis connection string (optional)
   dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
   ```
   
   If Redis is not available, the application automatically falls back to in-memory caching.

4. **Run the application**
   ```bash
   cd src/dafukSpin
   dotnet run
   ```

5. **Access the API**
   - **Swagger UI**: `https://localhost:7069` or `http://localhost:5244`
   - **API Base**: `http://localhost:5244/api`

## API Endpoints

### Base URL
```
http://localhost:5244/api
```

### Available Endpoints

#### User Anime Lists
- `GET /users/{username}/completed` - Get user's completed anime
- `GET /users/{username}/watching` - Get user's currently watching anime
- `GET /users/{username}/plan-to-watch` - Get user's plan to watch anime
- `GET /users/{username}/on-hold` - Get user's on hold anime
- `GET /users/{username}/dropped` - Get user's dropped anime
- `GET /users/{username}/animelist` - Get user's anime list with custom status filter

#### Anime Details & Search
- `GET /anime/{animeId}` - Get detailed anime information
- `GET /anime/search` - Search anime by query

#### Discovery & Rankings
- `GET /anime/ranking` - Get anime rankings by type
- `GET /anime/season/{year}/{season}` - Get seasonal anime
- `GET /anime/suggestions` - Get suggested anime (requires authentication)

#### Health Check
- `GET /health` - API health status

#### Cache Management
- `DELETE /api/cache/clear` - Clear all cached data (development/admin use)
- `GET /api/cache/stats` - Get cache statistics and performance metrics

#### Observability
- **Structured Logging**: All requests include correlation IDs for tracing
- **OpenTelemetry Metrics**: Request duration, cache hit rates, external API calls
- **Performance Monitoring**: Built-in process and runtime metrics

### Example Usage

**Get user's completed anime:**
```http
GET http://localhost:5244/api/users/testuser/completed
```

**Get anime details:**
```http
GET http://localhost:5244/api/anime/5114
```

**Search for anime:**
```http
GET http://localhost:5244/api/anime/search?query=naruto&limit=10
```

**Get seasonal anime:**
```http
GET http://localhost:5244/api/anime/season/2024/spring
```

### Response Format

All endpoints return JSON responses with consistent error handling and correlation tracking:

```json
{
  "data": [...],
  "paging": {
    "previous": "string", 
    "next": "string"
  }
}
```

**Response Headers:**
- `X-Correlation-ID`: Unique request identifier for tracing
- `Cache-Control`: Caching directives for client-side optimization

## Observability & Monitoring

### Structured Logging
- **Serilog Integration**: Rich structured logging with multiple sinks
- **Correlation IDs**: Every request gets a unique identifier for distributed tracing
- **Request Logging**: Automatic HTTP request/response logging with timing
- **Error Tracking**: Comprehensive error logging with context

### OpenTelemetry Tracing
- **Distributed Tracing**: Full request lifecycle tracking
- **External API Monitoring**: MyAnimeList API call tracing
- **Performance Metrics**: Request duration, cache performance, system metrics
- **Export Options**: Console output (development), OTLP for production

### Cache Monitoring
- **Multi-Tier Strategy**: Redis primary with memory cache fallback
- **Performance Metrics**: Hit rates, response times, eviction statistics
- **Health Checks**: Automatic fallback on Redis unavailability
- **Management Endpoints**: Clear cache and view statistics

## Caching Strategy

### Redis Caching (Primary)
- **External Cache**: Redis server for shared caching across instances
- **Configurable TTL**: Different expiration times for different data types
- **Fallback Handling**: Automatic memory cache fallback on Redis failure

### Memory Caching (Fallback)
- **Local Cache**: In-process caching when Redis unavailable
- **Performance**: Fastest access for single-instance deployments
- **Development**: Works without external dependencies

## Development

### Project Structure

The solution uses **centralized package management** with Directory.Build.props and Directory.Packages.props for consistent versioning across all projects:

```
dafukSpin/
├── Directory.Build.props          # Common build properties
├── Directory.Packages.props       # Centralized package versions
├── src/dafukSpin/                 # Main API project
│   ├── Services/                  # Business logic and integrations
│   ├── Models/                    # DTOs and data models
│   ├── Extensions/                # Minimal API endpoint mappings
│   └── Program.cs                 # Application startup
└── test/dafukSpin.Tests/          # Comprehensive test suite
    ├── Integration/               # API integration tests
    ├── Services/                  # Service unit tests
    └── Models/                    # Model validation tests
```

### Package Management

This project uses **centralized package management** for enterprise-grade dependency management:

- **Directory.Build.props**: Common build properties, target framework, and global settings
- **Directory.Packages.props**: All package versions defined in one place (25+ packages)
- **Project Files**: Only contain package references without versions
- **Benefits**: Consistent versions, simplified updates, reduced conflicts

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test test/dafukSpin.Tests
```

### Building
```bash
# Build the solution
dotnet build

# Build for release
dotnet build -c Release

# Clean build
dotnet clean && dotnet build
```

### Docker
```bash
# Build Docker image
docker build -t dafukspin .

# Run with environment variables
docker run -p 8080:8080 \
  -e MyAnimeList__ClientId="your-client-id" \
  -e Redis__ConnectionString="your-redis-connection" \
  dafukspin

# Run with Redis via Docker Compose
docker-compose up
```

## Configuration

### User Secrets Configuration
Development credentials are stored in .NET User Secrets:

```json
{
  "MyAnimeList:ClientId": "your-client-id",
  "MyAnimeList:ClientSecret": "your-client-secret",
  "Redis:ConnectionString": "localhost:6379"
}
```

### Application Settings
Public configuration in `appsettings.json`:

```json
{
  "MyAnimeList": {
    "BaseUrl": "https://api.myanimelist.net/v2"
  },
  "Serilog": {
    "MinimumLevel": "Information",
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/dafukspin-.txt" } }
    ]
  },
  "Cache": {
    "DefaultTtlMinutes": 30,
    "AnimeTtlMinutes": 60,
    "UserListTtlMinutes": 15
  }
}
```

### Production Configuration
For production deployment:
- Use Azure Key Vault, AWS Secrets Manager, or similar secure storage
- Set environment variables: `MyAnimeList__ClientId`, `Redis__ConnectionString`
- Configure OpenTelemetry exporters for monitoring systems
- Never store credentials in configuration files

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)
- `MyAnimeList__ClientId`: MyAnimeList API Client ID (production)
- `MyAnimeList__ClientSecret`: MyAnimeList API Client Secret (production)
- `Redis__ConnectionString`: Redis connection string (optional)
- `OTEL_EXPORTER_OTLP_ENDPOINT`: OpenTelemetry endpoint for production monitoring

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the coding guidelines in `.github/copilot-instructions.md`
4. Write tests for new features
5. Commit your changes (`git commit -m 'Add amazing feature'`)
6. Push to the branch (`git push origin feature/amazing-feature`)
7. Open a Pull Request

## License
This project is licensed under the MIT License - see the [LICENSE.txt](LICENSE.txt) file for details.