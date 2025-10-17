# dafukSpin

A modern .NET 8 Web API that integrates with the official MyAnimeList API to provide anime data services. Built with minimal APIs, enterprise-grade observability, caching, and comprehensive error handling.

## 🚀 Features

- **🎯 Official MyAnimeList API Integration**: Uses the official v2 API with proper authentication
- **⚡ Minimal APIs Architecture**: Modern ASP.NET Core endpoint mapping with clean organization  
- **📊 Comprehensive Anime Data**: User lists, anime details, search, rankings, and seasonal data
- **🔧 Refit HTTP Client**: Type-safe API client with automatic JSON serialization
- **🛡️ Polly Resilience**: Built-in retry policies and circuit breakers
- **📈 Enterprise Observability**: Structured logging with Serilog and OpenTelemetry tracing
- **⚡ Multi-Tier Caching**: Redis with memory cache fallback for optimal performance
- **🔍 Correlation ID Tracking**: Request tracing across all services and dependencies
- **📦 Centralized Package Management**: Directory.Build.props and Directory.Packages.props for consistent versioning
- ** Swagger Documentation**: Interactive API documentation at root URL
- **🔐 User Secrets**: Secure credential storage for development
- **🐳 Docker Support**: Multi-stage Docker builds for containerized deployment

## 🏗️ Architecture Highlights

- **Modern C# Patterns**: Records, pattern matching, file-scoped namespaces
- **Clean Separation**: Services, models, and endpoint mappings in separate layers
- **Type Safety**: Strongly-typed models with proper JSON serialization
- **Error Handling**: Comprehensive logging and graceful error responses
- **Observability First**: Full request tracing, metrics, and structured logging
- **Performance Optimized**: Multi-tier caching with Redis and memory fallback

### 🎨 Design Patterns
- **Service Layer Pattern**: Clean abstraction over external API calls with `IMyAnimeListService`
- **Refit Pattern**: Type-safe HTTP client generation with `IMyAnimeListApi`
- **Options Pattern**: Configuration binding with validation and user secrets
- **Dependency Injection**: Clean service registration and lifecycle management
- **Record Types**: Immutable data structures with modern C# syntax
- **Sealed Classes**: Performance optimization and clear inheritance intent
- **Minimal APIs**: Route mapping through extension methods

## 🛠️ Getting Started

### Prerequisites
- .NET 8 SDK or later
- MyAnimeList API credentials (Client ID)
- Redis server (optional - falls back to memory cache)
- Internet connection (for MyAnimeList API access)
- Docker (optional, for containerized deployment)

### 📦 Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/phuc4real/dafukSpin.git
   cd dafukSpin
   ```

2. **🔑 Configure MyAnimeList API Credentials**
   
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
   
   **⚠️ Important:** Never commit API credentials to source control. They are securely stored in user secrets.

<<<<<<< HEAD
3. **Configure Redis (Optional)**
   
   For optimal performance, configure Redis caching:
   
   ```bash
   # Set Redis connection string (optional)
   dotnet user-secrets set "Redis:ConnectionString" "localhost:6379"
   ```
   
   If Redis is not available, the application automatically falls back to in-memory caching.

4. **Run the application**
=======
3. **🚀 Run the application**
>>>>>>> 7575298b283896e616649c3724ba47320234788e
   ```bash
   cd src/dafukSpin
   dotnet run
   ```

<<<<<<< HEAD
5. **Access the API**
=======
4. **🌐 Access the API**
>>>>>>> 7575298b283896e616649c3724ba47320234788e
   - **Swagger UI**: `https://localhost:7069` or `http://localhost:5244`
   - **API Base**: `http://localhost:5244/api`
   - **Health Check**: `http://localhost:5244/health`

## 📋 API Endpoints

### Base URL
```
http://localhost:5244/api/anime
```

### 👤 User Anime Lists
| Endpoint | Method | Description | Example |
|----------|--------|-------------|---------|
| `/users/{username}/anime` | GET | Get user's anime list with filters | `/users/testuser/anime?status=watching` |
| `/users/{username}/anime/completed` | GET | Get user's completed anime | `/users/testuser/anime/completed` |
| `/users/{username}/anime/watching` | GET | Get user's currently watching anime | `/users/testuser/anime/watching` |
| `/users/{username}/anime/plan-to-watch` | GET | Get user's plan to watch anime | `/users/testuser/anime/plan-to-watch` |
| `/users/{username}/anime/on-hold` | GET | Get user's on hold anime | `/users/testuser/anime/on-hold` |
| `/users/{username}/anime/dropped` | GET | Get user's dropped anime | `/users/testuser/anime/dropped` |

### 🎬 Anime Details & Search
| Endpoint | Method | Description | Example |
|----------|--------|-------------|---------|
| `/details/{animeId}` | GET | Get detailed anime information | `/details/5114` |
| `/search` | GET | Search anime by query | `/search?query=naruto&limit=10` |

### 🏆 Discovery & Rankings
| Endpoint | Method | Description | Example |
|----------|--------|-------------|---------|
| `/ranking` | GET | Get anime rankings by type | `/ranking?rankingType=all&limit=50` |
| `/seasonal/{year}/{season}` | GET | Get seasonal anime | `/seasonal/2024/spring` |
| `/suggestions` | GET | Get suggested anime | `/suggestions?limit=20` |

### 🔍 Query Parameters

#### Common Parameters
- `limit` (int): Maximum number of results (default: 100)
- `offset` (int): Offset for pagination (default: 0)
- `sort` (string): Sort order (`list_score`, `list_updated_at`, `anime_title`, `anime_start_date`, `anime_id`)

<<<<<<< HEAD
#### Cache Management
- `DELETE /api/cache/clear` - Clear all cached data (development/admin use)
- `GET /api/cache/stats` - Get cache statistics and performance metrics

#### Observability
- **Structured Logging**: All requests include correlation IDs for tracing
- **OpenTelemetry Metrics**: Request duration, cache hit rates, external API calls
- **Performance Monitoring**: Built-in process and runtime metrics

### Example Usage
=======
#### Status Filters
- `status` (string): Filter by status (`watching`, `completed`, `on_hold`, `dropped`, `plan_to_watch`)

#### Ranking Types
- `rankingType` (string): Type of ranking (`all`, `airing`, `upcoming`, `tv`, `ova`, `movie`, `special`, `bypopularity`, `favorite`)

#### Seasons
- `season` (string): Season name (`winter`, `spring`, `summer`, `fall`)

### 🔍 Example Usage
>>>>>>> 7575298b283896e616649c3724ba47320234788e

**Get user's completed anime:**
```http
GET http://localhost:5244/api/anime/users/testuser/anime/completed?limit=10&sort=list_updated_at
```

**Get anime details:**
```http
GET http://localhost:5244/api/anime/details/5114
```

**Search for anime:**
```http
GET http://localhost:5244/api/anime/search?query=attack%20on%20titan&limit=5
```

**Get seasonal anime:**
```http
GET http://localhost:5244/api/anime/seasonal/2024/spring?sort=anime_score&limit=25
```

**Get anime rankings:**
```http
GET http://localhost:5244/api/anime/ranking?rankingType=tv&limit=50
```

<<<<<<< HEAD
All endpoints return JSON responses with consistent error handling and correlation tracking:
=======
### 📊 Response Format
>>>>>>> 7575298b283896e616649c3724ba47320234788e

All endpoints return JSON responses with consistent structure:

#### Successful Response (List Data)
```json
{
  "data": [
    {
      "node": {
        "id": 5114,
        "title": "Fullmetal Alchemist: Brotherhood",
        "main_picture": {
          "medium": "https://cdn.myanimelist.net/images/anime/1208/94745.jpg",
          "large": "https://cdn.myanimelist.net/images/anime/1208/94745l.jpg"
        },
        "alternative_titles": {
          "synonyms": [],
          "en": "Fullmetal Alchemist: Brotherhood",
          "ja": "鋼の錬金術師 FULLMETAL ALCHEMIST"
        },
        "mean": 9.1,
        "rank": 1,
        "popularity": 3,
        "num_episodes": 64,
        "media_type": "TV",
        "rating": "R - 17+ (violence & profanity)",
        "genres": [
          {"id": 1, "name": "Action"},
          {"id": 2, "name": "Adventure"}
        ]
      },
      "list_status": {
        "status": "completed",
        "score": 10,
        "num_episodes_watched": 64,
        "updated_at": "2023-04-01T12:00:00Z"
      }
    }
  ],
  "paging": {
<<<<<<< HEAD
    "previous": "string", 
    "next": "string"
=======
    "previous": null,
    "next": "https://api.myanimelist.net/v2/users/testuser/animelist?offset=100"
>>>>>>> 7575298b283896e616649c3724ba47320234788e
  }
}
```

<<<<<<< HEAD
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
=======
#### Successful Response (Single Item)
```json
{
  "id": 5114,
  "title": "Fullmetal Alchemist: Brotherhood",
  "synopsis": "After a horrific alchemy experiment goes wrong...",
  "mean": 9.1,
  "rank": 1,
  "popularity": 3,
  "num_episodes": 64,
  "media_type": "TV",
  "status": "finished_airing",
  "rating": "R - 17+ (violence & profanity)",
  "studios": [
    {"id": 4, "name": "Bones"}
  ]
}
```

#### Error Response
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "User 'testuser' not found or has no completed anime"
}
```

### 🚨 HTTP Status Codes
- `200 OK`: Successful request
- `400 Bad Request`: Invalid parameters or malformed request
- `401 Unauthorized`: Authentication required
- `404 Not Found`: User or anime not found
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: Server error

## 🔧 Development

### 🧪 Running Tests
>>>>>>> 7575298b283896e616649c3724ba47320234788e
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
<<<<<<< HEAD
dotnet test test/dafukSpin.Tests
=======
dotnet test test/dafukSpin.Tests/
>>>>>>> 7575298b283896e616649c3724ba47320234788e
```

### 🏗️ Building
```bash
# Build the solution
dotnet build

# Build for release
dotnet build -c Release

<<<<<<< HEAD
# Clean build
=======
# Clean and rebuild
>>>>>>> 7575298b283896e616649c3724ba47320234788e
dotnet clean && dotnet build
```

### 🐳 Docker

#### Build and Run
```bash
# Build Docker image
docker build -t dafukspin .

<<<<<<< HEAD
# Run with environment variables
docker run -p 8080:8080 \
  -e MyAnimeList__ClientId="your-client-id" \
  -e Redis__ConnectionString="your-redis-connection" \
  dafukspin

# Run with Redis via Docker Compose
docker-compose up
=======
# Run container with environment variables
docker run -p 8080:8080 \
  -e MyAnimeList__ClientId="your-client-id" \
  -e MyAnimeList__ClientSecret="your-client-secret" \
  dafukspin
>>>>>>> 7575298b283896e616649c3724ba47320234788e
```

#### Docker Compose (Development)
```yaml
version: '3.8'
services:
  dafukspin-api:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - MyAnimeList__ClientId=your-client-id
      - MyAnimeList__ClientSecret=your-client-secret
```

### 🔍 Debugging
```bash
# Run in development mode with detailed logging
dotnet run --environment Development

# Run with specific logging level
dotnet run --environment Development -- --Logging:LogLevel:Default=Debug
```

## ⚙️ Configuration

### 🔐 User Secrets Configuration (Development)
Development credentials are stored in .NET User Secrets:

```json
{
  "MyAnimeList:ClientId": "your-client-id",
  "MyAnimeList:ClientSecret": "your-client-secret",
  "Redis:ConnectionString": "localhost:6379"
}
```

**Managing User Secrets:**
```bash
# List current secrets
dotnet user-secrets list

# Set a secret
dotnet user-secrets set "MyAnimeList:ClientId" "your-client-id"

# Remove a secret
dotnet user-secrets remove "MyAnimeList:ClientId"

# Clear all secrets
dotnet user-secrets clear
```

### 📄 Application Settings
Public configuration in `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
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

<<<<<<< HEAD
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
=======
### 🌐 Production Configuration
For production deployment, use secure credential storage:

#### Azure
```bash
# Using Azure Key Vault
az keyvault secret set --vault-name "your-vault" --name "MyAnimeList--ClientId" --value "your-client-id"
```
>>>>>>> 7575298b283896e616649c3724ba47320234788e

#### AWS
```bash
# Using AWS Secrets Manager
aws secretsmanager create-secret --name "dafukSpin/MyAnimeList" --secret-string '{"ClientId":"your-client-id","ClientSecret":"your-client-secret"}'
```

#### Environment Variables
```bash
# Linux/macOS
export MyAnimeList__ClientId="your-client-id"
export MyAnimeList__ClientSecret="your-client-secret"

# Windows
set MyAnimeList__ClientId=your-client-id
set MyAnimeList__ClientSecret=your-client-secret
```

### 🌍 Environment Variables Reference
| Variable | Description | Required | Default |
|----------|-------------|----------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | No | `Production` |
| `MyAnimeList__ClientId` | MyAnimeList API Client ID | Yes | - |
| `MyAnimeList__ClientSecret` | MyAnimeList API Client Secret | No | - |
| `MyAnimeList__BaseUrl` | MyAnimeList API base URL | No | `https://api.myanimelist.net/v2` |

## 🤝 Contributing

We welcome contributions! Please follow these guidelines:

### 📋 Development Guidelines
1. **Fork the repository** and create a feature branch
   ```bash
   git checkout -b feature/amazing-feature
   ```

2. **Follow coding standards** in `.github/copilot-instructions.md`
   - Use modern C# patterns (records, pattern matching, file-scoped namespaces)
   - Write comprehensive tests for new features
   - Follow the established architecture patterns

3. **Commit your changes** with descriptive messages
   ```bash
   git commit -m 'feat: Add amazing new feature for user anime statistics'
   ```

4. **Push to your branch** and open a Pull Request
   ```bash
   git push origin feature/amazing-feature
   ```

### 🧪 Testing Requirements
- Write unit tests for all new functionality
- Ensure all existing tests pass
- Maintain code coverage above 80%

### 📚 Documentation
- Update README.md for new features
- Add XML documentation for public APIs
- Update API.md for new endpoints

### 🏷️ Commit Convention
We follow [Conventional Commits](https://www.conventionalcommits.org/):
- `feat:` - New features
- `fix:` - Bug fixes
- `docs:` - Documentation changes
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance tasks

## 📜 License
This project is licensed under the **GNU Affero General Public License v3.0** - see the [LICENSE.txt](LICENSE.txt) file for details.

## 🙏 Acknowledgments
- [MyAnimeList](https://myanimelist.net/) for providing the official API
- [Refit](https://github.com/reactiveui/refit) for the excellent HTTP client library
- [Polly](https://github.com/App-vNext/Polly) for resilience and transient-fault handling

## 📞 Support
- 📧 **Issues**: [GitHub Issues](https://github.com/phuc4real/dafukSpin/issues)
- 📖 **Documentation**: Check the `docs/` folder
- 💬 **Discussions**: [GitHub Discussions](https://github.com/phuc4real/dafukSpin/discussions)