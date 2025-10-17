namespace dafukSpin.Endpoints;

/// <summary>
/// Interface for defining API endpoints that can be automatically mapped
/// </summary>
public interface IEndpoint
{
    /// <summary>
    /// Maps the endpoint routes to the application
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    void MapEndpoint(IEndpointRouteBuilder app);
}