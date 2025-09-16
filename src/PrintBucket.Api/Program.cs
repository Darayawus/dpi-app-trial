using PrintBucket.Common.Logging;
using Serilog;
using System.Reflection;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Optional: configure Prometheus metrics
// No services needed for basic prom-net middleware; you can add custom collectors here.

try
{
    var app = builder.Build();
    Log.Information($"Starting PrintBucket API Version {Assembly.GetExecutingAssembly().GetName().Version}");


    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("Swagger enabled in development environment");
    }

    // Use prometheus middleware to expose metrics
    // Exposes /metrics for Prometheus to scrape and /metrics-text and /metrics for text and content-type negotiation
    app.UseHttpMetrics();

    // Map scrape endpoint; default is /metrics
    app.MapMetrics();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ERROR: Unexpected error in PrintBucket API");
    throw;
}
finally
{
    Log.Information("PrintBucket API shutdown complete");
    Log.CloseAndFlush();
}
