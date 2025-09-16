using PrintBucket.Common.Logging;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Serilog
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
    Log.Information("API PrintBucket construida correctamente");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("Swagger habilitado en modo desarrollo");
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("API PrintBucket iniciando...");
    app.Run();
    Log.Information("API PrintBucket finalizada correctamente");
}
catch (Exception ex)
{
    Log.Fatal(ex, "La API se detuvo inesperadamente");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
