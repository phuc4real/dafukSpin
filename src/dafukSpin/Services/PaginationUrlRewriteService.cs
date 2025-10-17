using dafukSpin.Models;
using System.Web;

namespace dafukSpin.Services;

/// <summary>
/// Service implementation for rewriting MyAnimeList pagination URLs to use dafukSpin API endpoints
/// </summary>
public sealed class PaginationUrlRewriteService : IPaginationUrlRewriteService
{
    public MyAnimeListResponse<T> RewritePaginationUrls<T>(
        MyAnimeListResponse<T> response,
        string baseUrl,
        string currentEndpoint)
    {
        var rewrittenPaging = new Paging(
            RewriteUrl(response.Paging.Previous, baseUrl, currentEndpoint),
            RewriteUrl(response.Paging.Next, baseUrl, currentEndpoint)
        );

        return response with { Paging = rewrittenPaging };
    }

    public Dictionary<string, string> ExtractQueryParameters(string myAnimeListUrl)
    {
        var parameters = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(myAnimeListUrl) || !Uri.TryCreate(myAnimeListUrl, UriKind.Absolute, out var uri))
        {
            return parameters;
        }

        var queryString = uri.Query;
        if (string.IsNullOrEmpty(queryString))
        {
            return parameters;
        }

        var query = HttpUtility.ParseQueryString(queryString);
        foreach (string? key in query.AllKeys)
        {
            if (key is not null && query[key] is not null)
            {
                parameters[key] = query[key]!;
            }
        }

        return parameters;
    }

    private string? RewriteUrl(string? myAnimeListUrl, string baseUrl, string currentEndpoint)
    {
        if (string.IsNullOrWhiteSpace(myAnimeListUrl))
        {
            return null;
        }

        // Extract query parameters from the MyAnimeList URL
        var parameters = ExtractQueryParameters(myAnimeListUrl);

        if (parameters.Count == 0)
        {
            return null;
        }

        // Build the new URL with dafukSpin base URL and current endpoint
        var baseUri = new Uri(baseUrl.TrimEnd('/'));
        var endpointUri = new Uri(baseUri, currentEndpoint.TrimStart('/'));

        // Convert parameters to query string
        var queryParameters = parameters.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}");
        var queryString = string.Join("&", queryParameters);

        return $"{endpointUri}?{queryString}";
    }
}