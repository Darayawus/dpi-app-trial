using PrintBucket.Common.Logging;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
