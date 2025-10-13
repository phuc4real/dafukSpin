# MyAnimeList API Integration

This project includes comprehensive API endpoints to interact with MyAnimeList.net user data, providing access to completed anime, currently watching anime, full anime history, and plan to watch functionality.

## Features

- Get top 10 latest completed anime for any MyAnimeList user
- Get top 5 currently watching anime with progress tracking
- Get paginated anime history (all statuses)
- Get random anime from plan to watch list
- Comprehensive anime information including scores, rankings, genres, and images
- Error handling for various scenarios (user not found, private lists, API errors)
- Swagger documentation for easy API testing
- Health check endpoint

## API Endpoints

### 1. Get Top 10 Latest Completed Anime
```
GET /api/MyAnimeList/users/{username}/completed-anime
```

**Parameters:**
- `username` (string, required): The MyAnimeList username

**Response:**
Returns a list of the 10 most recently completed anime.

**Example Response:**
```json
[
  {
    "id": 16498,
    "title": "Attack on Titan",
    "englishTitle": "Attack on Titan",
    "imageUrl": "https://cdn.myanimelist.net/images/anime/10/47347l.jpg",
    "score": 9.0,
    "userScore": 10,
    "rank": 1,
    "popularity": 1,
    "numEpisodes": 25,
    "mediaType": "tv",
    "rating": "R",
    "genres": ["Action", "Drama", "Fantasy"],
    "completedAt": "2023-12-01T10:30:00Z",
    "finishDate": "2023-12-01"
  }
]
```

### 2. Get Top 5 Currently Watching Anime
```
GET /api/MyAnimeList/users/{username}/currently-watching
```

**Parameters:**
- `username` (string, required): The MyAnimeList username

**Response:**
Returns a list of the 5 most recently updated currently watching anime with progress information.

**Example Response:**
```json
[
  {
    "id": 21,
    "title": "One Piece",
    "englishTitle": "One Piece",
    "imageUrl": "https://cdn.myanimelist.net/images/anime/6/73245l.jpg",
    "score": 9.0,
    "userScore": 9,
    "rank": 25,
    "popularity": 3,
    "numEpisodes": null,
    "numEpisodesWatched": 1050,
    "mediaType": "tv",
    "rating": "PG-13",
    "genres": ["Action", "Adventure", "Comedy"],
    "studios": ["Toei Animation"],
    "isRewatching": false,
    "lastUpdated": "2023-12-15T18:22:00Z",
    "startDate": "2020-01-01",
    "progressPercentage": 0.0
  }
]
```

### 3. Get User Anime History (Paginated)
```
GET /api/MyAnimeList/users/{username}/history?page={page}&pageSize={pageSize}
```

**Parameters:**
- `username` (string, required): The MyAnimeList username
- `page` (int, optional): Page number (default: 1, minimum: 1)
- `pageSize` (int, optional): Items per page (default: 20, range: 1-100)

**Response:**
Returns paginated anime history including all statuses (completed, watching, on hold, dropped, plan to watch).

**Example Response:**
```json
{
  "data": [
    {
      "id": 5114,
      "title": "Fullmetal Alchemist: Brotherhood",
      "englishTitle": "Fullmetal Alchemist: Brotherhood",
      "imageUrl": "https://cdn.myanimelist.net/images/anime/1223/96541l.jpg",
      "score": 9.1,
      "userScore": 10,
      "rank": 1,
      "popularity": 5,
      "numEpisodes": 64,
      "mediaType": "tv",
      "rating": "R",
      "genres": ["Action", "Adventure", "Drama"],
      "studios": ["Bones"],
      "completedAt": "2023-11-15T14:30:00Z",
      "finishDate": "2023-11-15",
      "startDate": "2023-10-01",
      "numTimesRewatched": 1,
      "comments": "Amazing series, definitely rewatching!",
      "tags": ["masterpiece", "emotional"]
    }
  ],
  "pagination": {
    "currentPage": 1,
    "totalPages": 15,
    "totalItems": 300,
    "pageSize": 20,
    "hasNext": true,
    "hasPrevious": false
  }
}
```

### 4. Get Random Plan to Watch Anime
```
GET /api/MyAnimeList/users/{username}/random-plan-to-watch
```

**Parameters:**
- `username` (string, required): The MyAnimeList username

**Response:**
Returns a randomly selected anime from the user's plan to watch list.

**Example Response:**
```json
{
  "id": 33352,
  "title": "Violet Evergarden",
  "englishTitle": "Violet Evergarden",
  "imageUrl": "https://cdn.myanimelist.net/images/anime/3/88110l.jpg",
  "score": 8.5,
  "rank": 221,
  "popularity": 125,
  "numEpisodes": 13,
  "mediaType": "tv",
  "rating": "PG-13",
  "genres": ["Drama", "Fantasy", "Slice of Life"],
  "studios": ["Kyoto Animation"],
  "synopsis": "The Great War finally came to an end after four long years...",
  "startDate": "2018-01-11",
  "endDate": "2018-04-05",
  "source": "Novel",
  "priority": 1,
  "addedToListAt": "2023-06-20T12:15:00Z",
  "tags": ["kyoani", "must-watch"],
  "comments": "Heard great things about this!"
}
```

### 5. Health Check
```
GET /api/MyAnimeList/health
```

Returns the service status, timestamp, and available endpoints.

## Response Status Codes

All endpoints return the following status codes:

- **200 OK**: Successful request
- **400 Bad Request**: Invalid username, page, or pageSize parameters
- **401 Unauthorized**: MyAnimeList API authentication failed
- **404 Not Found**: User not found, has private list, or no data found
- **500 Internal Server Error**: Server configuration or processing error
- **502 Bad Gateway**: MyAnimeList API is unavailable

## Setup Instructions

### 1. MyAnimeList API Registration

Before using this API, you need to register an application with MyAnimeList:

1. Go to [MyAnimeList API](https://myanimelist.net/apiconfig)
2. Create a new Client ID
3. Note down your Client ID (you'll need this for configuration)

### 2. Configuration

Add your MyAnimeList Client ID to the configuration:

**appsettings.json:**
```json
{
  "MyAnimeList": {
    "ClientId": "your_actual_client_id_here"
  }
}
```

**For production, use environment variables or Azure Key Vault:**
```bash
export MyAnimeList__ClientId="your_actual_client_id_here"
```

### 3. Running the Application

```bash
cd src/dafukSpin
dotnet run
```

The API will be available at:
- Development: `https://localhost:7xxx` or `http://localhost:5xxx`
- Swagger UI: Available at the root URL when running in development mode

## Usage Examples

### Using curl

```bash
# Get completed anime for user "username"
curl -X GET "https://localhost:7xxx/api/MyAnimeList/users/username/completed-anime"

# Get currently watching anime
curl -X GET "https://localhost:7xxx/api/MyAnimeList/users/username/currently-watching"

# Get anime history with pagination
curl -X GET "https://localhost:7xxx/api/MyAnimeList/users/username/history?page=1&pageSize=10"

# Get random plan to watch anime
curl -X GET "https://localhost:7xxx/api/MyAnimeList/users/username/random-plan-to-watch"

# Health check
curl -X GET "https://localhost:7xxx/api/MyAnimeList/health"
```

### Using PowerShell

```powershell
# Get completed anime
Invoke-RestMethod -Uri "https://localhost:7xxx/api/MyAnimeList/users/username/completed-anime" -Method Get

# Get currently watching anime
Invoke-RestMethod -Uri "https://localhost:7xxx/api/MyAnimeList/users/username/currently-watching" -Method Get

# Get anime history
Invoke-RestMethod -Uri "https://localhost:7xxx/api/MyAnimeList/users/username/history?page=1&pageSize=20" -Method Get

# Get random plan to watch anime
Invoke-RestMethod -Uri "https://localhost:7xxx/api/MyAnimeList/users/username/random-plan-to-watch" -Method Get

# Health check
Invoke-RestMethod -Uri "https://localhost:7xxx/api/MyAnimeList/health" -Method Get
```

## Response Models

### CompletedAnimeDto
- **Id**: MyAnimeList anime ID
- **Title**: Primary title of the anime
- **EnglishTitle**: English title (if available)
- **ImageUrl**: URL to the anime's cover image
- **Score**: Average score on MyAnimeList (0-10)
- **UserScore**: User's personal score (0-10)
- **Rank**: Global ranking on MyAnimeList
- **Popularity**: Popularity ranking
- **NumEpisodes**: Number of episodes
- **MediaType**: Type of media (tv, movie, ova, etc.)
- **Rating**: Age rating (G, PG, R, etc.)
- **Genres**: List of genre names
- **CompletedAt**: When the user marked it as completed
- **FinishDate**: User's finish date (if provided)

### CurrentlyWatchingAnimeDto
Includes all CompletedAnimeDto properties plus:
- **NumEpisodesWatched**: Number of episodes watched so far
- **Studios**: List of animation studios
- **IsRewatching**: Whether the user is rewatching
- **LastUpdated**: When the entry was last updated
- **StartDate**: When the user started watching
- **ProgressPercentage**: Watching progress percentage (0-100)

### AnimeHistoryDto
Comprehensive anime information including:
- All basic anime information
- **NumTimesRewatched**: How many times the user has rewatched
- **Comments**: User's personal comments
- **Tags**: User-defined tags
- **StartDate** and **FinishDate**: User's watching dates

### PlanToWatchAnimeDto
Includes detailed anime information plus:
- **Synopsis**: Full anime synopsis
- **Source**: Source material (manga, novel, etc.)
- **Priority**: User's priority level
- **AddedToListAt**: When added to plan to watch list
- **Tags**: User-defined tags
- **Comments**: User's notes about wanting to watch

### PagedAnimeHistoryResponse
- **Data**: List of AnimeHistoryDto items
- **Pagination**: Pagination information including current page, total pages, and navigation flags

## Error Handling

The API includes comprehensive error handling:

- **User Not Found**: Returns 404 with a descriptive message
- **Private Lists**: Handled as user not found (MyAnimeList limitation)
- **API Rate Limiting**: Returns 502 with retry suggestion
- **Configuration Errors**: Returns 500 with configuration guidance
- **Invalid Input**: Returns 400 with validation message
- **Empty Results**: Returns appropriate empty responses with 200 or 404 status

## Logging

The application logs all API interactions:

- Info: Successful requests and responses
- Warning: Invalid inputs and not-found scenarios
- Error: API failures and unexpected errors
- Debug: Full API responses (in development)

## Limitations

1. **Public Lists Only**: Only works with public MyAnimeList anime lists
2. **Rate Limiting**: Subject to MyAnimeList API rate limits
3. **Authentication**: Requires a valid MyAnimeList Client ID
4. **Data Freshness**: Data is fetched in real-time but may be cached by MyAnimeList
5. **Pagination**: History endpoint uses client-side pagination due to MyAnimeList API limitations

## Future Enhancements

Potential improvements could include:
- Caching for improved performance
- Support for different sorting options
- Batch requests for multiple users
- Additional anime details and statistics
- User authentication for private lists
- Real-time notifications for list updates
- Advanced filtering and search capabilities

## Troubleshooting

### Common Issues

1. **401 Unauthorized**
   - Check that your MyAnimeList Client ID is correctly configured
   - Verify the Client ID is valid and active

2. **404 User Not Found**
   - Verify the username exists on MyAnimeList
   - Check if the user's anime list is public

3. **404 No Data Found (Random Plan to Watch)**
   - User has no anime in their plan to watch list
   - Verify the user has a public plan to watch list

4. **502 Bad Gateway**
   - MyAnimeList API may be temporarily unavailable
   - Check MyAnimeList service status

5. **500 Internal Server Error**
   - Check application logs for specific error details
   - Verify configuration is complete

### Support

For additional support or bug reports, please check the application logs and ensure your MyAnimeList API configuration is correct.