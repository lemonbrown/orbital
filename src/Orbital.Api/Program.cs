using Microsoft.EntityFrameworkCore;
using Orbital.Api.Endpoints;
using Orbital.Api.Middleware;
using Orbital.Application;
using Orbital.Infrastructure;
using Orbital.Infrastructure.Persistence;
using Orbital.Infrastructure.Persistence.Seed;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Apply migrations and seed on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrbitalDbContext>();
    await db.Database.MigrateAsync();

    if (app.Environment.IsDevelopment())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        await DatabaseSeeder.SeedAsync(db, logger);
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Map endpoint groups
app.MapGroup("/api/auth").MapAuthEndpoints().WithTags("Auth");
app.MapGroup("/api/sites").MapSiteEndpoints().WithTags("Sites");
app.MapGroup("/api/rings").MapRingEndpoints().WithTags("Rings");
app.MapGroup("/api/navigate").MapNavigationEndpoints().WithTags("Navigation");

app.Run();
