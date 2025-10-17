using System.Reflection;
using dafukSpin.Endpoints;

namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for automatic endpoint registration
/// </summary>
public static class EndpointExtensions
{
    /// <summary>
    /// Automatically registers all endpoints that implement IEndpoint interface
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        // Get all types that implement IEndpoint
        var endpointTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) &&
                       !t.IsInterface &&
                       !t.IsAbstract)
            .ToList();

        foreach (var endpointType in endpointTypes)
        {
            // Create instance of the endpoint
            if (Activator.CreateInstance(endpointType) is IEndpoint endpoint)
            {
                endpoint.MapEndpoint(app);
            }
        }

        return app;
    }
}