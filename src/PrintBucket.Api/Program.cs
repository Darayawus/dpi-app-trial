using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using PrintBucket.AWS;
using PrintBucket.AWS.Services;
using PrintBucket.Common.Logging;
using Prometheus;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Initialize Serilog
SerilogLogger.Initialize();
builder.Host.UseSerilog();

// Configure AWS (read options from appsettings / environment)
// Get region (fallback to eu-west-1)
var region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS:Region"] ?? "eu-west-1");

// Try resolving credentials from profile in development
AWSCredentials? resolvedCredentials = null;
if (builder.Environment.IsDevelopment())
{
    var profile = builder.Configuration["AWS:Profile"];
    if (!string.IsNullOrWhiteSpace(profile))
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials(profile, out var profCreds))
            resolvedCredentials = profCreds;
    }
}

// Add services to the container.
// Register DynamoDB client using resolved credentials (if any) or default resolution
builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    return resolvedCredentials is not null
        ? new AmazonDynamoDBClient(resolvedCredentials, region)
        : new AmazonDynamoDBClient(region);
});

// Register S3 client
builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    return resolvedCredentials is not null
        ? new AmazonS3Client(resolvedCredentials, region)
        : new AmazonS3Client(region);
});

builder.Services.AddScoped<IBucketService, BucketService>();
builder.Services.AddScoped<IS3StorageService, S3StorageService>();
builder.Services.AddScoped<IImageService, ImageService>();
// Register AI services
builder.Services.AddScoped<IBedrockImageAnalyzer, BedrockImageAnalyzer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

try
{
    var app = builder.Build();
    Log.Information($"Starting PrintBucket API Version {Assembly.GetExecutingAssembly().GetName().Version}");

    // Let the SDK resolve credentials automatically (env vars, profile, or IAM role)
    AWSCredentials? creds = null;

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        Log.Information("Swagger enabled in development environment");       
    }

    // Use prometheus middleware to expose metrics
    // Exposes /metrics for Prometheus to scrape and /metrics-text and /metrics for text and content-type negotiation
    // Optional: configure Prometheus metrics
    // No services needed for basic prom-net middleware; you can add custom collectors here.
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
