using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PrintBucket.Models;
using System.Net.Http.Headers;

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

            try
            {
                // ImagesController expects a single file per POST to: POST /images/upload/{bucketId}
                foreach (var file in Files)
                {
                    using var content = new MultipartFormDataContent();
                    using var stream = file.OpenReadStream();
                    var streamContent = new StreamContent(stream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType ?? "application/octet-stream");
                    content.Add(streamContent, "file", file.FileName);

                    var response = await _httpClient.PostAsync($"images/upload/{id}", content);

                    if (!response.IsSuccessStatusCode)
                    {
                        var body = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Upload failed for {File} to bucket {BucketId}: {Status} {Body}", file.FileName, id, response.StatusCode, body);
                        ModelState.AddModelError(string.Empty, $"Upload failed for {file.FileName}: {response.StatusCode}");
                    }
                    else
                    {
                        _logger.LogInformation("Uploaded {File} to bucket {BucketId}", file.FileName, id);
                    }
                }

                if (!ModelState.IsValid)
                    return Page();

                // Reload page to show updated images
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