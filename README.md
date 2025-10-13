# dafukSpin

A modern .NET 8 Web API that integrates with the official MyAnimeList API to provide anime data services. Built with minimal APIs, Refit HTTP client library, and comprehensive error handling.

## Features

- **Official MyAnimeList API Integration**: Uses the official v2 API with proper authentication
- **Minimal APIs Architecture**: Modern ASP.NET Core endpoint mapping with clean organization
- **Comprehensive Anime Data**: User lists, anime details, search, rankings, and seasonal data
- **Refit HTTP Client**: Type-safe API client with automatic JSON serialization
- **Polly Resilience**: Built-in retry policies and circuit breakers
- **Swagger Documentation**: Interactive API documentation at root URL
- **User Secrets**: Secure credential storage for development

## Architecture Highlights

- **Modern C# Patterns**: Records, pattern matching, file-scoped namespaces
- **Clean Separation**: Services, models, and endpoint mappings in separate layers
- **Type Safety**: Strongly-typed models with proper JSON serialization
- **Error Handling**: Comprehensive logging and graceful error responses

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

3. **Run the application**
   ```bash
   cd src/dafukSpin
   dotnet run
   ```

4. **Access the API**
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

All endpoints return JSON responses with consistent error handling:

```json
{
  "data": [...],
  "paging": {
    "previous": "string",
    "next": "string"
  }
}
```

## Development

### Running Tests
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Building
```bash
# Build the solution
dotnet build

# Build for release
dotnet build -c Release
```

### Docker
```bash
# Build Docker image
docker build -t dafukspin .

# Run container (Note: User secrets not available in container)
docker run -p 8080:8080 -e MyAnimeList__ClientId="your-client-id" dafukspin
```

## Configuration

### User Secrets Configuration
Development credentials are stored in .NET User Secrets:

```json
{
  "MyAnimeList:ClientId": "your-client-id",
  "MyAnimeList:ClientSecret": "your-client-secret"
}
```

### Application Settings
Public configuration in `appsettings.json`:

```json
{
  "MyAnimeList": {
    "BaseUrl": "https://api.myanimelist.net/v2"
  }
}
```

### Production Configuration
For production deployment:
- Use Azure Key Vault, AWS Secrets Manager, or similar secure storage
- Set environment variables: `MyAnimeList__ClientId` and `MyAnimeList__ClientSecret`
- Never store credentials in configuration files

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)
- `MyAnimeList__ClientId`: MyAnimeList API Client ID (production)
- `MyAnimeList__ClientSecret`: MyAnimeList API Client Secret (production)

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