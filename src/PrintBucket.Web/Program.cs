using PrintBucket.Common.Logging;
using Serilog;
using Prometheus;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddRazorPages();
// Add Controllers
builder.Services.AddControllers();

// Localización: indicar carpeta Resources
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("es") };

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use prometheus middleware 
//TODO: this is a trial, no protection here right now
app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Activar middleware de localización usando la configuración registrada
var locOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(locOptions);

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // Mapear controladores

app.Run();
