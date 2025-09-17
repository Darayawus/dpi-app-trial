using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PrintBucket.AWS.Services;
using PrintBucket.Graphics;
using PrintBucket.Models;
using Serilog;

namespace PrintBucket.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagesController : ControllerBase
    {
        private readonly ILogger<ImagesController> _logger;
        private readonly IS3StorageService _s3Storage;
        private readonly IBucketService _bucketService;
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;

        public ImagesController(
            ILogger<ImagesController> logger,
            IS3StorageService s3Storage,
            IBucketService bucketService,
            IImageService imageService,
            IConfiguration configuration)
        {
            _logger = logger;
            _s3Storage = s3Storage;
            _bucketService = bucketService;
            _imageService = imageService;
            _configuration = configuration;
        }

        [HttpPost("upload/{bucketId}")]
        public async Task<IActionResult> Upload(string bucketId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var bucketName = _configuration.GetValue<string>("AWS:S3:BucketName");

                // Verificar que el bucket existe
                var bucket = await _bucketService.GetBucketByIdAsync(bucketId);
                if (bucket == null)
                    return NotFound("Bucket not found");

                if (!ImageProcessor.IsNetVipsAvailable())
                    return StatusCode(500, "Image processing service not available");

                using (var stream = file.OpenReadStream())
                {
                    var uploadedKeys = await _s3Storage.UploadImageWithVersionsAsync(bucketName, bucketId, stream, file.FileName);

                    // Crear registro en DynamoDB (asume orden: original, large, small)
                    var record = new ImageRecord
                    {
                        BucketId = bucketId,
                        OriginalKey = uploadedKeys.ElementAtOrDefault(0) ?? string.Empty,
                        LargeKey = uploadedKeys.ElementAtOrDefault(1) ?? string.Empty,
                        SmallKey = uploadedKeys.ElementAtOrDefault(2) ?? string.Empty,
                        FileName = file.FileName,
                        ContentType = file.ContentType ?? "image/jpeg",
                        Size = file.Length,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _imageService.AddImageAsync(record);

                    _logger.LogInformation("Images uploaded and record created for bucket {BucketId}: {Files}", bucketId, string.Join(", ", uploadedKeys));

                    return Ok(new
                    {
                        id = record.Id,
                        originalName = file.FileName,
                        files = uploadedKeys
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing image for bucket {BucketId}", bucketId);
                return StatusCode(500, "Error processing image");
            }
        }

        // New endpoint: list images for a bucket
        [HttpGet("bucket/{bucketId}")]
        public async Task<IActionResult> GetByBucket(string bucketId)
        {
            try
            {
                // verify bucket exists (optional)
                var bucket = await _bucketService.GetBucketByIdAsync(bucketId);
                if (bucket == null)
                    return NotFound("Bucket not found");

                var images = await _imageService.GetImagesByBucketAsync(bucketId, limit: 100);
                images = images.OrderBy(x => x.CreatedAt).ToList();
                return Ok(images);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images for bucket {BucketId}", bucketId);
                return StatusCode(500, "Error retrieving images");
            }
        }
    }
}