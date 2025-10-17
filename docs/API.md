# 📚 API Documentation

## dafukSpin MyAnimeList API

### 🌟 Overview
The dafukSpin API provides access to MyAnimeList user data through the official MyAnimeList API v2, delivering a clean, modern REST API interface. All endpoints return JSON data and follow standard HTTP status codes.

### 🔐 Authentication
**Authentication Required!** The MyAnimeList API requires Client ID authentication configured securely via user secrets or environment variables.

**Authentication Method:** API Key (Header)
- **Header Name:** `X-MAL-CLIENT-ID`
- **Header Value:** Your MyAnimeList Client ID

### 🌐 Base URL
```
http://localhost:5244/api/anime
```

### 📋 Response Format
- **Successful responses**: Return JSON data with consistent structure
- **Error responses**: Follow the Problem Details standard (RFC 7807)
- **Content-Type**: `application/json`
- **Character encoding**: UTF-8

### 🔗 Pagination URL Rewriting
All pagination URLs in API responses are automatically rewritten to use dafukSpin endpoints instead of direct MyAnimeList URLs. This provides:

- **API Consistency**: All URLs point to dafukSpin endpoints
- **Request Control**: All pagination requests go through dafukSpin for caching and logging
- **Transparent Experience**: Users work with a unified API interface

**Example:**
- **Original**: `https://api.myanimelist.net/v2/users/testuser/animelist?offset=100`
- **Rewritten**: `http://localhost:5244/api/anime/users/testuser/anime/completed?offset=100`

---

## 📖 Endpoints Reference

### 👤 1. User Anime Lists

#### Get User's Completed Anime

**Endpoint:** `GET /users/{username}/anime/completed`

**Description:** Retrieves the completed anime list for a specific MyAnimeList user.

**Path Parameters:**
- `username` (required, string): MyAnimeList username

**Query Parameters:**
- `sort` (optional, string): Sort order
  - `list_score` - By user score
  - `list_updated_at` - By last updated (default)
  - `anime_title` - By anime title
  - `anime_start_date` - By anime start date
  - `anime_id` - By anime ID
- `limit` (optional, integer): Maximum number of results (1-1000, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/users/testuser/anime/completed?sort=list_updated_at&limit=10
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
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
          {"id": 2, "name": "Adventure"},
          {"id": 8, "name": "Drama"},
          {"id": 10, "name": "Fantasy"}
        ]
      },
      "list_status": {
        "status": "completed",
        "score": 10,
        "num_episodes_watched": 64,
        "updated_at": "2023-04-01T12:00:00Z",
        "finish_date": "2023-03-31"
      }
    }
  ],
  "paging": {
    "previous": null,
    "next": "http://localhost:5244/api/anime/users/testuser/anime/completed?offset=100"
  }
}
```

**Error Responses:**
- `400 Bad Request`: Invalid parameters or malformed request
- `404 Not Found`: User not found or has no completed anime
- `429 Too Many Requests`: Rate limit exceeded
- `500 Internal Server Error`: API or server error

---

#### Get User's Currently Watching Anime

**Endpoint:** `GET /users/{username}/anime/watching`

**Description:** Retrieves the currently watching anime list for a specific MyAnimeList user.

**Path Parameters:**
- `username` (required, string): MyAnimeList username

**Query Parameters:**
- `sort` (optional, string): Sort order (same options as completed anime)
- `limit` (optional, integer): Maximum number of results (1-1000, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/users/testuser/anime/watching?limit=5
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
```json
{
  "data": [
    {
      "node": {
        "id": 16498,
        "title": "Shingeki no Kyojin",
        "main_picture": {
          "medium": "https://cdn.myanimelist.net/images/anime/10/47347.jpg",
          "large": "https://cdn.myanimelist.net/images/anime/10/47347l.jpg"
        },
        "alternative_titles": {
          "synonyms": [],
          "en": "Attack on Titan",
          "ja": "進撃の巨人"
        },
        "mean": 8.7,
        "rank": 15,
        "popularity": 1,
        "num_episodes": 25,
        "media_type": "TV",
        "rating": "R - 17+ (violence & profanity)",
        "genres": [
          {"id": 1, "name": "Action"},
          {"id": 8, "name": "Drama"},
          {"id": 10, "name": "Fantasy"}
        ]
      },
      "list_status": {
        "status": "watching",
        "score": 9,
        "num_episodes_watched": 15,
        "is_rewatching": false,
        "updated_at": "2024-01-15T10:30:00Z",
        "start_date": "2024-01-01"
      }
    }
  ],
  "paging": {
    "previous": null,
    "next": null
  }
}
```

---

#### Additional User List Endpoints

**Plan to Watch Anime**
- **Endpoint:** `GET /users/{username}/anime/plan-to-watch`
- **Description:** Get user's plan to watch anime list
- **Parameters:** Same as completed anime endpoint

**On Hold Anime**
- **Endpoint:** `GET /users/{username}/anime/on-hold`
- **Description:** Get user's on hold anime list
- **Parameters:** Same as completed anime endpoint

**Dropped Anime**
- **Endpoint:** `GET /users/{username}/anime/dropped`
- **Description:** Get user's dropped anime list
- **Parameters:** Same as completed anime endpoint

**Custom Anime List**
- **Endpoint:** `GET /users/{username}/anime`
- **Description:** Get user's anime list with custom status filter
- **Additional Parameters:**
  - `status` (optional, string): Filter by status (`watching`, `completed`, `on_hold`, `dropped`, `plan_to_watch`)

---

### 🎬 2. Anime Details & Search

#### Get Anime Details

**Endpoint:** `GET /details/{animeId}`

**Description:** Retrieves detailed information about a specific anime by its MyAnimeList ID.

**Path Parameters:**
- `animeId` (required, integer): MyAnimeList anime ID

**Example Request:**
```http
GET /api/anime/details/5114
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
```json
{
  "id": 5114,
  "title": "Fullmetal Alchemist: Brotherhood",
  "main_picture": {
    "medium": "https://cdn.myanimelist.net/images/anime/1208/94745.jpg",
    "large": "https://cdn.myanimelist.net/images/anime/1208/94745l.jpg"
  },
  "alternative_titles": {
    "synonyms": ["Fullmetal Alchemist (2009)", "FMA Brotherhood"],
    "en": "Fullmetal Alchemist: Brotherhood",
    "ja": "鋼の錬金術師 FULLMETAL ALCHEMIST"
  },
  "start_date": "2009-04-05",
  "end_date": "2010-07-04",
  "synopsis": "After a horrific alchemy experiment goes wrong in the Elric household...",
  "mean": 9.1,
  "rank": 1,
  "popularity": 3,
  "num_episodes": 64,
  "media_type": "TV",
  "status": "finished_airing",
  "rating": "R - 17+ (violence & profanity)",
  "genres": [
    {"id": 1, "name": "Action"},
    {"id": 2, "name": "Adventure"},
    {"id": 8, "name": "Drama"},
    {"id": 10, "name": "Fantasy"},
    {"id": 27, "name": "Shounen"}
  ],
  "studios": [
    {"id": 4, "name": "Bones"}
  ]
}
```

---

#### Search Anime

**Endpoint:** `GET /search`

**Description:** Search for anime using a text query.

**Query Parameters:**
- `query` (required, string): Search query text
- `limit` (optional, integer): Maximum number of results (1-100, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/search?query=attack%20on%20titan&limit=5
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
```json
{
  "data": [
    {
      "node": {
        "id": 16498,
        "title": "Shingeki no Kyojin",
        "main_picture": {
          "medium": "https://cdn.myanimelist.net/images/anime/10/47347.jpg",
          "large": "https://cdn.myanimelist.net/images/anime/10/47347l.jpg"
        },
        "alternative_titles": {
          "en": "Attack on Titan",
          "ja": "進撃の巨人"
        },
        "mean": 8.7,
        "media_type": "TV"
      }
    }
  ],
  "paging": {
    "previous": null,
    "next": "http://localhost:5244/api/anime/search?query=attack%20on%20titan&offset=100"
  }
}
```

---

### 🏆 3. Discovery & Rankings

#### Get Anime Rankings

**Endpoint:** `GET /ranking`

**Description:** Retrieves anime rankings by type.

**Query Parameters:**
- `rankingType` (optional, string): Type of ranking (default: `all`)
  - `all` - All anime rankings
  - `airing` - Currently airing anime
  - `upcoming` - Upcoming anime
  - `tv` - TV series
  - `ova` - OVA
  - `movie` - Movies
  - `special` - Specials
  - `bypopularity` - By popularity
  - `favorite` - Most favorited
- `limit` (optional, integer): Maximum number of results (1-500, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/ranking?rankingType=tv&limit=10
Host: localhost:5244
Accept: application/json
```

**Example Response:** `200 OK`
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
        "mean": 9.1,
        "media_type": "TV"
      },
      "ranking": {
        "rank": 1,
        "previous_rank": 1
      }
    }
  ],
  "paging": {
    "previous": null,
    "next": "http://localhost:5244/api/anime/ranking?rankingType=tv&offset=100"
  }
}
```

---

#### Get Seasonal Anime

**Endpoint:** `GET /seasonal/{year}/{season}`

**Description:** Retrieves anime for a specific year and season.

**Path Parameters:**
- `year` (required, integer): Year (1917 or later)
- `season` (required, string): Season name
  - `winter` - Winter season (January-March)
  - `spring` - Spring season (April-June)
  - `summer` - Summer season (July-September)
  - `fall` - Fall season (October-December)

**Query Parameters:**
- `sort` (optional, string): Sort order
  - `anime_score` - By anime score
  - `anime_num_list_users` - By number of users
- `limit` (optional, integer): Maximum number of results (1-500, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/seasonal/2024/spring?sort=anime_score&limit=10
Host: localhost:5244
Accept: application/json
```

---

#### Get Suggested Anime

**Endpoint:** `GET /suggestions`

**Description:** Retrieves suggested anime (requires user authentication).

**Query Parameters:**
- `limit` (optional, integer): Maximum number of results (1-100, default: 100)
- `offset` (optional, integer): Offset for pagination (default: 0)

**Example Request:**
```http
GET /api/anime/suggestions?limit=10
Host: localhost:5244
Accept: application/json
X-MAL-CLIENT-ID: your-client-id
```

---

## 🔍 Data Models

### AnimeNode
Core anime information structure:
```typescript
{
  id: number
  title: string
  main_picture?: {
    medium?: string
    large?: string
  }
  alternative_titles?: {
    synonyms?: string[]
    en?: string
    ja?: string
  }
  start_date?: string
  end_date?: string
  synopsis?: string
  mean?: number
  rank?: number
  popularity?: number
  num_episodes?: number
  media_type?: string
  status?: string
  rating?: string
  genres?: Array<{id: number, name: string}>
  studios?: Array<{id: number, name: string}>
}
```

### ListStatus
User's anime list status information:
```typescript
{
  status: string
  score?: number
  num_episodes_watched?: number
  is_rewatching?: boolean
  updated_at?: string
  start_date?: string
  finish_date?: string
  priority?: number
  num_times_rewatched?: number
  rewatch_value?: number
  tags?: string[]
  comments?: string
}
```

---

## 🚨 Error Handling

### HTTP Status Codes
| Code | Description | Common Causes |
|------|-------------|---------------|
| `200` | OK | Successful request |
| `400` | Bad Request | Invalid parameters, malformed request |
| `401` | Unauthorized | Missing or invalid authentication |
| `404` | Not Found | User or anime not found |
| `429` | Too Many Requests | Rate limit exceeded |
| `500` | Internal Server Error | Server error, API unavailable |

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "User 'testuser' not found or has no completed anime",
  "instance": "/api/users/testuser/completed"
}
```

---

## 📊 Data Source & Limitations

### Data Source
- **MyAnimeList API v2**: Official MyAnimeList REST API
- **Real-time Data**: Data is fetched live from MyAnimeList
- **Rate Limits**: Subject to MyAnimeList API rate limits

### API Limitations
- **Rate Limiting**: MyAnimeList enforces rate limits on API calls
- **Authentication**: Client ID required for all requests
- **User Privacy**: Private user lists are not accessible
- **Data Freshness**: Data reflects current MyAnimeList database state

---

## 🚀 Getting Started

1. **Get API Credentials**: Visit [MyAnimeList API Configuration](https://myanimelist.net/apiconfig)
2. **Configure Secrets**: Set up your Client ID using user secrets or environment variables
3. **Start the API**: Run `dotnet run` from the project directory
4. **Access Swagger UI**: Navigate to `http://localhost:5244/`
5. **Test Endpoints**: Use the interactive Swagger documentation
6. **Make Requests**: Use any HTTP client to call the endpoints

### Quick Test
```bash
# Health check
curl http://localhost:5244/health

# Get user's completed anime
curl "http://localhost:5244/api/users/testuser/completed?limit=5"

# Search for anime
curl "http://localhost:5244/api/anime/search?query=naruto&limit=3"
```
