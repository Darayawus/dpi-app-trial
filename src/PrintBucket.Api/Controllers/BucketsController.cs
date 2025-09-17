using Microsoft.AspNetCore.Mvc;
using PrintBucket.AWS.Services;
using PrintBucket.Models;

namespace PrintBucket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BucketsController : ControllerBase
    {
        private readonly IBucketService _bucketService;
        private readonly ILogger<BucketsController> _logger;

        public BucketsController(IBucketService bucketService, ILogger<BucketsController> logger)
        {
            _bucketService = bucketService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<Bucket>> CreateBucket([FromBody] CreateBucketRequest request)
        {
            try
            {
                var bucket = await _bucketService.CreateBucketAsync(request.Email, request.Name);
                _logger.LogInformation("Bucket created successfully for {Email}", request.Email);
                return CreatedAtAction(nameof(GetById), new { id = bucket.Id }, bucket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bucket for {Email}", request.Email);
                return StatusCode(500, "Error creating bucket");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Bucket>> GetById(string id)
        {
            var bucket = await _bucketService.GetBucketByIdAsync(id);
            if (bucket == null)
                return NotFound();

            return Ok(bucket);
        }

        [HttpGet("access/{accessCode}")]
        public async Task<ActionResult<Bucket>> GetByAccessCode(string accessCode)
        {
            var space = await _bucketService.GetBucketByAccessCodeAsync(accessCode);
            if (space == null)
                return NotFound();

            return Ok(space);
        }
    }

    public class CreateBucketRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}