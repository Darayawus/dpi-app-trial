using PrintBucket.Common.Logging;
using Serilog;
using Prometheus;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using PrintBucket.Web.Resources;

var builder = WebApplication.CreateBuilder(args);

// Inicializar Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Localización: configurar antes que otros servicios
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configurar servicios de localización
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "es", "en" };
    options.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
    
    options.FallBackToParentCultures = true;
    options.FallBackToParentUICultures = true;
});

// Add services to the container.
builder.Services.AddRazorPages()
    .AddViewLocalization()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(SharedResource));
    });

//Add Controllers
builder.Services.AddControllers();
builder.Services.AddHttpClient("PrintBucketApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5003/");
});

var app = builder.Build();

// Configurar localización ANTES de otros middleware
app.UseRequestLocalization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//TODO: Prometheus protect
app.UseHttpMetrics();
app.MapMetrics();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
