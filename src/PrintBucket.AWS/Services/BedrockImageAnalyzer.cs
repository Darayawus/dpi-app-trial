using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using System.Text.Json;
using Serilog;
using PrintBucket.Common.Logging;

namespace PrintBucket.AWS.Services
{
    public interface IBedrockImageAnalyzer
    {
        Task<ImageAnalysisResult> AnalyzeImageAsync(string base64Image);
    }

    public class BedrockImageAnalyzer : IBedrockImageAnalyzer
    {
        private readonly AmazonBedrockRuntimeClient _bedrockClient;
        private readonly ILogger _logger;

        public BedrockImageAnalyzer(string awsAccessKey, string awsSecretKey, string awsRegion)
        {
            var config = new AmazonBedrockRuntimeConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsRegion)
            };

            _bedrockClient = new AmazonBedrockRuntimeClient(awsAccessKey, awsSecretKey, config);
            _logger = Log.ForContext<BedrockImageAnalyzer>();
        }

        public async Task<ImageAnalysisResult> AnalyzeImageAsync(string base64Image)
        {
            try
            {
                _logger.Information("Starting image analysis with Bedrock Claude-3");

                // Log image size for debugging
                var imageSize = base64Image.Length * 3 / 4; // Approximate size in bytes
                _logger.Debug("Processing image of size: {ImageSizeKB}KB", imageSize / 1024);

                // Crear el prompt para Claude-3
                var prompt = new
                {
                    anthropic_version = "claude-3-sonnet-20240229",
                    max_tokens = 1024,
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new
                                {
                                    type = "image",
                                    source = new
                                    {
                                        type = "base64",
                                        media_type = "image/jpeg",
                                        data = base64Image
                                    }
                                },
                                new
                                {
                                    type = "text",
                                    text = "Describe detalladamente esta imagen. Incluye informaci�n sobre el contenido, colores dominantes, composici�n y cualquier detalle relevante."
                                }
                            }
                        }
                    }
                };

                _logger.Debug("Sending request to Bedrock API");

                var request = new InvokeModelRequest
                {
                    ModelId = "anthropic.claude-3-sonnet-20240229",
                    ContentType = "application/json",
                    Accept = "application/json",
                    Body = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(prompt))
                };

                var sw = System.Diagnostics.Stopwatch.StartNew();
                var response = await _bedrockClient.InvokeModelAsync(request);
                sw.Stop();

                _logger.Information("Received response from Bedrock API in {ElapsedMs}ms", sw.ElapsedMilliseconds);

                using var reader = new StreamReader(response.Body);
                var jsonResponse = await reader.ReadToEndAsync();
                
                var result = new ImageAnalysisResult
                {
                    Success = true,
                    Description = JsonDocument.Parse(jsonResponse)
                        .RootElement
                        .GetProperty("content")
                        .GetProperty("text")
                        .GetString() ?? "No se pudo obtener descripci�n",
                    RawResponse = jsonResponse
                };

                _logger.Information("Image analysis completed successfully. Description length: {DescriptionLength} characters",
                    result.Description.Length);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error analyzing image with Bedrock: {ErrorMessage}", ex.Message);
                
                return new ImageAnalysisResult
                {
                    Success = false,
                    Error = ex.Message,
                    RawResponse = ex.ToString()
                };
            }
        }
    }

    public class ImageAnalysisResult
    {
        public bool Success { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Error { get; set; }
        public string RawResponse { get; set; } = string.Empty;
    }
}