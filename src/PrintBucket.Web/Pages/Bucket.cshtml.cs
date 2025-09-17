using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PrintBucket.Models;

namespace PrintBucket.Web.Pages
{
    public class BucketModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BucketModel> _logger;

        public BucketModel(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<BucketModel> logger)
        {
            _httpClient = httpClientFactory.CreateClient("PrintBucketApi");
            _configuration = configuration;
            _logger = logger;
        }

        public PrintBucket.Models.Bucket? Bucket { get; private set; }
        public List<ImageRecord> Images { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"buckets/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Bucket = await response.Content.ReadFromJsonAsync<PrintBucket.Models.Bucket>();
                    
                    // Obtener imágenes
                    var imagesResponse = await _httpClient.GetAsync($"images/bucket/{id}");
                    if (imagesResponse.IsSuccessStatusCode)
                    {
                        Images = await imagesResponse.Content.ReadFromJsonAsync<List<ImageRecord>>() ?? new();
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting bucket {Id}", id);
                return StatusCode(500);
            }
        }

        [BindProperty]
        public List<IFormFile> Files { get; set; } = new();

        public async Task<IActionResult> OnPostUploadAsync(string id)
        {
            if (Files == null || Files.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No files selected.");
                return Page();
            }

            var allowedExt = new[] { ".jpg", ".jpeg" };

            try
            {
                foreach (var file in Files)
                {
                    var ext = Path.GetExtension(file.FileName)?.ToLowerInvariant() ?? string.Empty;
                    var contentType = file.ContentType ?? string.Empty;

                    if (!allowedExt.Contains(ext) || !(contentType.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase) || contentType.Equals("image/jpg", StringComparison.OrdinalIgnoreCase)))
                    {
                        ModelState.AddModelError(string.Empty, $"File '{file.FileName}' is not a JPG image.");
                        return Page();
                    }

                    using var content = new MultipartFormDataContent();
                    using var stream = file.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "image/jpeg");
                    content.Add(streamContent, "file", file.FileName);

                    var response = await _httpClient.PostAsync($"images/upload/{id}", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Upload failed for {File} to bucket {BucketId}: {Status} {Body}", file.FileName, id, response.StatusCode, body);
                        ModelState.AddModelError(string.Empty, $"Upload failed for {file.FileName}: {response.StatusCode}");
                        return Page();
                    }

                    _logger.LogInformation("Uploaded {File} to bucket {BucketId}", file.FileName, id);
                }

                return RedirectToPage(new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading files to bucket {Id}", id);
                ModelState.AddModelError(string.Empty, "An error occurred while uploading files.");
                return Page();
            }
        }
    }
}