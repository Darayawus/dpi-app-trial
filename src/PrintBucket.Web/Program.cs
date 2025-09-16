using PrintBucket.Common.Logging;
using Serilog;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
//Add Controllers
builder.Services.AddControllers(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use prometheus middleware
//TODO: Add to readme.MD that metrics security is disabled as this is a trial
app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Mapear controladores

app.Run();
