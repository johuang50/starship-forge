using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using StarshipForge.Api.Hubs;
using StarshipForge.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});
builder.Services.AddSingleton<HardwareCatalog>();
builder.Services.AddSingleton<StarshipDesignAgent>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowFrontend");

// Map SignalR hub
app.MapHub<DesignHub>("/designHub");

// API endpoint for design requests
app.MapPost("/api/design", async ([FromBody] DesignRequest body, StarshipDesignAgent agent) =>
{
    await agent.ProcessDesignRequest(body.Request);
    return Results.Ok("Design request submitted");
});

app.Run();

public class DesignRequest
{
    public string Request { get; set; } = string.Empty;
}
