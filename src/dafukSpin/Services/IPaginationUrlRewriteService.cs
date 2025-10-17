using dafukSpin.Models;

namespace dafukSpin.Services;

/// <summary>
/// Service for rewriting MyAnimeList pagination URLs to use dafukSpin API endpoints
/// </summary>
public interface IPaginationUrlRewriteService
{
    /// <summary>
    /// Rewrites pagination URLs in a MyAnimeList response to use dafukSpin API endpoints
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="response">The original MyAnimeList response</param>
    /// <param name="baseUrl">The base URL of the dafukSpin API</param>
    /// <param name="currentEndpoint">The current API endpoint path</param>
    /// <returns>A new response with rewritten pagination URLs</returns>
    MyAnimeListResponse<T> RewritePaginationUrls<T>(
        MyAnimeListResponse<T> response,
        string baseUrl,
        string currentEndpoint);

    /// <summary>
    /// Extracts query parameters from a MyAnimeList pagination URL
    /// </summary>
    /// <param name="myAnimeListUrl">The MyAnimeList URL to parse</param>
    /// <returns>Dictionary of query parameters</returns>
    Dictionary<string, string> ExtractQueryParameters(string myAnimeListUrl);
}