using System.Text.Json.Serialization;
using GenshinAccountAnalyzer.Analyzer;
using GenshinAccountAnalyzer.Api.Middleware;
using GenshinAccountAnalyzer.Application;
using GenshinAccountAnalyzer.Calculator;
using GenshinAccountAnalyzer.Infrastructure;
using GenshinAccountAnalyzer.Parser;
using GenshinAccountAnalyzer.Report;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Application layers.
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddParsers();
builder.Services.AddAnalyzers();
builder.Services.AddCalculator();
builder.Services.AddReports();

// Web/API services. Serialize enums as their string names for a readable, stable contract.
builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allows the Angular SPA (served from a different origin in development) to call this API.
const string SpaCorsPolicy = "SpaClient";
string[] allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    is { Length: > 0 } configured
        ? configured
        : ["http://localhost:4200"];

builder.Services.AddCors(options =>
{
    options.AddPolicy(SpaCorsPolicy, policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

WebApplication app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(SpaCorsPolicy);
app.UseHttpsRedirection();
app.MapControllers();

app.Run();

/// <summary>Program entry point marker, exposed so integration tests can reference the API host.</summary>
public partial class Program;
