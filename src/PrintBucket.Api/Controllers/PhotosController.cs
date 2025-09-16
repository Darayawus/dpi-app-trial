using Microsoft.AspNetCore.Mvc;
using NetVips;
using PrintBucket.Graphics;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

namespace PrintBucket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private const string UploadDirectory = "uploads";

        public ImagesController(ILogger<ImagesController> logger)
        {
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }

                // Verificar que NetVips está disponible
                if (!ImageProcessor.IsNetVipsAvailable())
                {
                    return StatusCode(500, "Image processing service not available");
                }

                // Crear directorio si no existe
                if (!Directory.Exists(UploadDirectory))
                {
                    Directory.CreateDirectory(UploadDirectory);
                }

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(UploadDirectory, fileName);

                // Guardar el archivo temporalmente
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Procesar con NetVips
                using (var originalImage = NetVips.Image.NewFromFile(filePath))
                {
                    NetVips.Image processedImage = originalImage;
                    // Ejemplo: redimensionar a máximo 1200px de ancho manteniendo proporción
                    if (originalImage.Width > 1200)
                    {
                        processedImage = originalImage.Resize((double)1200 / originalImage.Width);
                    }

                    // Guardar la imagen procesada
                    var processedPath = Path.Combine(UploadDirectory, $"processed_{fileName}");
                    processedImage.WriteToFile(processedPath);

                    _logger.LogInformation("Image processed successfully: {FileName}", fileName);

                    return Ok(new
                    {
                        originalName = file.FileName,
                        savedAs = fileName,
                        processedFile = $"processed_{fileName}",
                        size = processedImage.Width + "x" + processedImage.Height
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image");
                return StatusCode(500, "Error processing image");
            }
        }
    }
}