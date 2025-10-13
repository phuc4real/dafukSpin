# dafukSpin

## Overview
dafukSpin is a .NET 8 ASP.NET Core Web API that integrates with the MyAnimeList API to provide anime data services. The project uses **Minimal APIs** architecture with a clean separation between services, models, and endpoint mappings.

## Features
- 🎯 **MyAnimeList API Integration**: Fetches anime data from official MyAnimeList API
- 📋 **Top Completed Anime**: Get top-rated completed anime from MyAnimeList
- 📺 **Top Airing Anime**: Get currently airing anime with high ratings
- 📚 **Top Anime Rankings**: Paginated top anime listings from MyAnimeList
- 🎲 **Random Upcoming Anime**: Get random anime from upcoming season
- 🔍 **Anime Search**: Search for anime by title
- 📖 **Anime Details**: Get detailed information for specific anime
- 🚀 **Minimal APIs**: Modern ASP.NET Core minimal API architecture
- 🔓 **No Authentication**: Direct access without API keys required
- 📖 **OpenAPI/Swagger**: Full API documentation and testing interface
- 🐳 **Docker Support**: Ready for containerized deployment

## Architecture

### Technology Stack
- **.NET 8** - Latest .NET framework
- **ASP.NET Core** - Web framework with minimal APIs
- **System.Text.Json** - High-performance JSON serialization
- **HttpClient** - External API integration
- **Swagger/OpenAPI** - API documentation

### Project Structure
```
src/dafukSpin/
├── Extensions/        # Minimal API endpoint mappings
├── Models/           # DTOs and data models
├── Services/         # Business logic and external API integration
└── Program.cs        # Application entry point and configuration
```

### Key Patterns
- **Minimal APIs**: No controllers, endpoints defined via extension methods
- **Dependency Injection**: HttpClient injection for external API calls
- **Records for DTOs**: Immutable data structures with modern C# syntax
- **Sealed Classes**: Performance optimization and clear inheritance intent
- **File-scoped Namespaces**: Modern C# namespace declarations

## Getting Started

### Prerequisites
- .NET 8 SDK
- Internet connection (for MyAnimeList API access)

### Setup
1. **Clone the repository**
   ```bash
   git clone https://github.com/phuc4real/dafukSpin.git
   cd dafukSpin
   ```

2. **Run the application**
   ```bash
   cd src/dafukSpin
   dotnet run
   ```

3. **Access the API**
   - **Swagger UI**: `https://localhost:7069` or `http://localhost:5244`
   - **API Base**: `http://localhost:5244/api/anime`

**Configuration required!** The API uses the official MyAnimeList API which requires Client ID authentication.

## API Endpoints

### Base URL
```
http://localhost:5244/api/myanimelist
```

### Available Endpoints

#### Get User's Completed Anime
```http
GET /users/{username}/completed
```
Returns the completed anime list for a specific MyAnimeList user.

#### Get User's Currently Watching Anime
```http
GET /users/{username}/watching
```
Returns the currently watching anime list for a specific MyAnimeList user.

#### Get User's Plan to Watch Anime
```http
GET /users/{username}/plan-to-watch
```
Returns the plan to watch anime list for a specific MyAnimeList user.

#### Get User's Anime List
```http
GET /users/{username}/animelist?status={status}&sort={sort}&limit={limit}&offset={offset}
```
Returns anime list for a specific user with custom status filter and pagination.

#### Get Random Plan to Watch Anime
```http
GET /users/{username}/plan-to-watch/random
```
Returns a random anime from the user's plan to watch list.

#### Get User's Anime History
```http
GET /users/{username}/history?page={page}&pageSize={pageSize}
```
Returns paginated anime history for a specific user.

### Example Usage

**Get user's completed anime:**
```http
GET http://localhost:5244/api/myanimelist/users/testuser/completed
```

**Response:**
```json
[
  {
    "id": 5114,
    "title": "Fullmetal Alchemist: Brotherhood",
    "englishTitle": "Fullmetal Alchemist: Brotherhood",
    "imageUrl": "https://cdn.myanimelist.net/images/anime/1208/94745l.jpg",
    "score": 9.1,
    "userScore": 10,
    "rank": 1,
    "popularity": 3,
    "numEpisodes": 64,
    "mediaType": "TV",
    "rating": "R - 17+ (violence & profanity)",
    "genres": ["Action", "Adventure", "Drama", "Fantasy"],
    "completedAt": "2023-04-01T12:00:00Z",
    "finishDate": "2023-03-31"
  }
]
```

**Get user's currently watching anime:**
```http
GET http://localhost:5244/api/myanimelist/users/testuser/watching
```

**Get user's anime history:**
```http
GET http://localhost:5244/api/myanimelist/users/testuser/history?page=1&pageSize=10
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

# Run container
docker run -p 8080:8080 dafukspin
```

## Configuration

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Environment (Development/Production)

### API Keys Required
The MyAnimeList API requires Client ID authentication. Configure your Client ID in the appsettings.json file.

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