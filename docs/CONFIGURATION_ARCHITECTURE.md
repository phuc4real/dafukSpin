# Configuration Architecture

This project uses extension methods to organize configuration into logical groups, keeping `Program.cs` clean and maintainable.

## Configuration Extensions

### JsonExtensions
**File**: `Extensions/JsonExtensions.cs`
- `AddDafukSpinJson()` - Configures JSON serialization for MyAnimeList API compatibility

### CachingExtensions
**File**: `Extensions/CachingExtensions.cs`
- `AddDafukSpinCaching()` - Configures memory caching for improved performance

### RefitExtensions  
**File**: `Extensions/RefitExtensions.cs`
- `AddMyAnimeListApi()` - Configures Refit HTTP client with authentication, retry policies, and service registration

### CorsExtensions
**File**: `Extensions/CorsExtensions.cs`
- `AddDafukSpinCors(params string[] allowedOrigins)` - Configures CORS policies
- `UseDafukSpinCors()` - Enables CORS middleware

### SwaggerExtensions
**File**: `Extensions/SwaggerExtensions.cs`
- `AddDafukSpinSwagger()` - Configures Swagger/OpenAPI documentation
- `UseDafukSpinSwagger()` - Enables Swagger UI in development

### ApplicationExtensions
**File**: `Extensions/ApplicationExtensions.cs`
- `AddDafukSpinServices(params string[] allowedOrigins)` - Registers all services in one call (includes caching)
- `UseDafukSpinMiddleware()` - Configures all middleware in correct order

## Clean Program.cs

The refactored `Program.cs` is now extremely concise:

```csharp
using dafukSpin.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add all dafukSpin services
builder.Services.AddDafukSpinServices();

var app = builder.Build();

// Configure middleware pipeline
app.UseDafukSpinMiddleware();

// Map all endpoints
app.MapEndpoints();

app.Run();

public partial class Program { }
```

## Benefits

- **🧹 Clean**: `Program.cs` reduced from 120+ lines to ~15 lines
- **📦 Modular**: Each configuration area is self-contained
- **🔧 Configurable**: Extension methods accept parameters for customization
- **🔄 Reusable**: Extensions can be used in other projects or tests
- **📚 Documented**: Each extension method has XML documentation
- **🎯 Focused**: Single responsibility per extension class
- **🛠️ Maintainable**: Configuration changes are isolated to specific files

## Customization Examples

### Custom CORS Origins
```csharp
builder.Services.AddDafukSpinServices(
    "https://myapp.com", 
    "https://www.myapp.com"
);
```

### Individual Configuration
```csharp
// For fine-grained control
builder.Services.AddDafukSpinJson();
builder.Services.AddMyAnimeListApi();
builder.Services.AddDafukSpinCors("https://custom-domain.com");
builder.Services.AddDafukSpinSwagger();
```

## Testing Benefits

Extensions make unit testing configuration much easier:

```csharp
[Test]
public void Should_Configure_Json_Options()
{
    var services = new ServiceCollection();
    services.AddDafukSpinJson();
    
    var serviceProvider = services.BuildServiceProvider();
    // Assert configuration is correct
}
```