# API Documentation

## dafukSpin MyAnimeList API

### Overview
The dafukSpin API provides access to MyAnimeList user data through the official MyAnimeList API, delivering a clean, modern REST API interface. All endpoints return JSON data and follow standard HTTP status codes.

### Authentication
Authentication required! The MyAnimeList API requires Client ID authentication configured in appsettings.json.

### Base URL
```
http://localhost:5244/api/myanimelist
```

### Response Format
All successful responses return JSON data. Error responses follow the Problem Details standard (RFC 7807).

---

## Endpoints

### 1. Get User's Completed Anime

**Endpoint:** `GET /users/{username}/completed`

**Description:** Retrieves the completed anime list for a specific MyAnimeList user.

**Path Parameters:**
- `username` (required): MyAnimeList username

**Example Request:**
```http
GET /api/myanimelist/users/testuser/completed
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
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

**Error Responses:**
- `400 Bad Request`: Invalid parameters
- `401 Unauthorized`: Authentication failed
- `404 Not Found`: User not found
- `500 Internal Server Error`: API or server error

---

### 2. Get User's Currently Watching Anime

**Endpoint:** `GET /users/{username}/watching`

**Description:** Retrieves the currently watching anime list for a specific MyAnimeList user.

**Path Parameters:**
- `username` (required): MyAnimeList username

**Example Request:**
```http
GET /api/myanimelist/users/testuser/watching
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
```json
[
  {
    "id": 16498,
    "title": "Attack on Titan",
    "englishTitle": "Attack on Titan",
    "imageUrl": "https://cdn.myanimelist.net/images/anime/10/47347l.jpg",
    "score": 8.7,
    "userScore": 9,
    "rank": 15,
    "popularity": 1,
    "numEpisodes": 25,
    "mediaType": "TV",
    "rating": "R - 17+ (violence & profanity)",
    "genres": ["Action", "Drama", "Fantasy"],
    "completedAt": null,
    "finishDate": null
  }
]
```

---

## Additional Endpoints

For more endpoint details, see the available endpoints:
- `GET /users/{username}/plan-to-watch` - Get user's plan to watch list
- `GET /users/{username}/animelist` - Get user's anime list with filters
- `GET /users/{username}/plan-to-watch/random` - Get random plan to watch anime  
- `GET /users/{username}/history` - Get user's anime history (paginated)

---

## Data Source

All data is sourced from:
- **MyAnimeList API v2**: Official MyAnimeList REST API
- **MyAnimeList**: Original data source for all anime information
- **Real-time**: Data is fetched live from MyAnimeList API

---

## Getting Started

1. **Configure API Keys**: Set up your MyAnimeList Client ID in appsettings.json
2. **Start the API**: Run `dotnet run` from the project directory
3. **Access Swagger UI**: Navigate to `http://localhost:5244/` 
4. **Test Endpoints**: Use the interactive Swagger documentation
5. **Make Requests**: Use any HTTP client to call the endpoints
