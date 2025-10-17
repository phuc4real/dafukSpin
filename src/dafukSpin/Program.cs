using dafukSpin.Extensions;
using Serilog;

// Create the builder but don't use the built-in logging
var builder = WebApplication.CreateBuilder(args);

// Clear default logging providers and use Serilog
builder.Logging.ClearProviders();

// Add all dafukSpin services
builder.Services.AddDafukSpinServices(builder.Configuration, builder.Environment);

// Use Serilog as the logging provider
builder.Host.UseSerilog();

var app = builder.Build();

// Configure middleware pipeline
app.UseDafukSpinMiddleware();

// Map all endpoints
app.MapEndpoints();

app.Run();

// Make Program accessible for integration testing
public partial class Program { }
