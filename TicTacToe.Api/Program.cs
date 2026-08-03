using System.Text.Json.Serialization;
using TicTacToe.Api.Middleware;
using TicTacToe.Core.Engine;
using TicTacToe.Core.Interfaces;
using TicTacToe.Core.Repositories;
using TicTacToe.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Tic Tac Toe API",
        Version = "v1",
        Description = "REST API backing the Tic Tac Toe assessment (.NET 8)."
    });
});

// Angular dev server runs on http://localhost:4200 by default.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularClient", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddSingleton<IGameRepository, GameRepository>();
builder.Services.AddSingleton<IGameEngine, GameEngine>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IScoreboardService, ScoreboardService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Tic Tac Toe API v1");
    });
}


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseCors("AngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
