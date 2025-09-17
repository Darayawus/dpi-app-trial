using Microsoft.AspNetCore.Mvc;
using PrintBucket.AWS.Services;
using System.Text.RegularExpressions;

namespace PrintBucket.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IBedrockImageAnalyzer _imageAnalyzer;
        private readonly ILogger<AIController> _logger;

        public AIController(IBedrockImageAnalyzer imageAnalyzer, ILogger<AIController> logger)
        {
            _imageAnalyzer = imageAnalyzer;
            _logger = logger;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest("No se ha proporcionado ninguna imagen");
                }

                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                var base64Image = Convert.ToBase64String(imageBytes);

                var result = await _imageAnalyzer.AnalyzeImageAsync(base64Image);

                if (!result.Success)
                {
                    _logger.LogError("Error al analizar la imagen: {Error}", result.Error);
                    return StatusCode(500, new { error = "Error al analizar la imagen", details = result.Error });
                }

                return Ok(new
                {
                    description = result.Description,
                    success = result.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar la solicitud de análisis de imagen");
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}