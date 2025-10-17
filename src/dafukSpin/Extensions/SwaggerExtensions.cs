namespace dafukSpin.Extensions;

/// <summary>
/// Extension methods for configuring Swagger/OpenAPI documentation
/// </summary>
public static class SwaggerExtensions
{
    /// <summary>
    /// Configures Swagger/OpenAPI documentation for the dafukSpin API
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddDafukSpinSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "dafukSpin API",
                Version = "v1",
                Description = "API for anime data via MyAnimeList Official API - Using Minimal APIs with Refit and Client ID Authentication",
                Contact = new Microsoft.OpenApi.Models.OpenApiContact
                {
                    Name = "dafukSpin",
                    Url = new Uri("https://github.com/phuc4real/dafukSpin")
                }
            });

            // Add XML comments if available
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            // Add authentication requirement info to Swagger
            options.AddSecurityDefinition("MAL-ClientId", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Name = "X-MAL-CLIENT-ID",
                Description = "MyAnimeList API Client ID for authentication"
            });
        });

        return services;
    }

    /// <summary>
    /// Configures Swagger UI middleware for development environment
    /// </summary>
    /// <param name="app">The web application</param>
    /// <returns>The web application for chaining</returns>
    public static WebApplication UseDafukSpinSwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "dafukSpin API v1");
                options.RoutePrefix = string.Empty; // Makes Swagger UI available at root
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
                options.DefaultModelsExpandDepth(-1); // Hide model schemas by default
            });
        }

        return app;
    }
}