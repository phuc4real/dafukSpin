using dafukSpin.Services;
using dafukSpin.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Remove AddControllers() since we're using minimal APIs
// builder.Services.AddControllers();

// Add HttpClient for MyAnimeList service
builder.Services.AddHttpClient<IMyAnimeListService, MyAnimeListService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "dafukSpin API",
        Version = "v1",
        Description = "API for MyAnimeList integration and other services - Now using Minimal APIs"
    });
    
    // Add XML comments if needed
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "dafukSpin API v1");
        options.RoutePrefix = string.Empty; // Makes Swagger UI available at root
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map minimal API endpoints instead of controllers
app.MapMyAnimeListEndpoints();

app.Run();
