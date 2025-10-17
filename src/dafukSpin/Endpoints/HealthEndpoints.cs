using dafukSpin.Endpoints;

namespace dafukSpin.Endpoints;

/// <summary>
/// Health check endpoint
/// </summary>
public sealed class HealthEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
            .WithName("HealthCheck")
            .WithSummary("Health check endpoint")
            .WithDescription("Returns the health status of the API")
            .WithTags("System")
            .Produces(200);
    }
}