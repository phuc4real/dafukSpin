using dafukSpin.Models;
using dafukSpin.Services;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for pagination URL rewriting
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Rewrites MyAnimeList pagination URLs to use dafukSpin API endpoints
    /// </summary>
    /// <typeparam name="T">The type of data in the response</typeparam>
    /// <param name="response">The MyAnimeList response</param>
    /// <param name="rewriteService">The pagination URL rewrite service</param>
    /// <param name="httpContext">The current HTTP context</param>
    /// <param name="currentEndpoint">The current API endpoint path</param>
    /// <returns>A new response with rewritten pagination URLs</returns>
    public static MyAnimeListResponse<T> RewritePaginationUrls<T>(
        this MyAnimeListResponse<T> response,
        IPaginationUrlRewriteService rewriteService,
        HttpContext httpContext,
        string currentEndpoint)
    {
        var baseUrl = GetBaseUrl(httpContext);
        return rewriteService.RewritePaginationUrls(response, baseUrl, currentEndpoint);
    }

    /// <summary>
    /// Gets the base URL from the current HTTP context
    /// </summary>
    /// <param name="httpContext">The HTTP context</param>
    /// <returns>The base URL (e.g., "https://localhost:7069" or "http://api.example.com")</returns>
    private static string GetBaseUrl(HttpContext httpContext)
    {
        var request = httpContext.Request;
        return $"{request.Scheme}://{request.Host}";
    }
}