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
                var response = await _httpClient.GetAsync($"api/buckets/{id}");
                if (response.IsSuccessStatusCode)
                {
                    Bucket = await response.Content.ReadFromJsonAsync<PrintBucket.Models.Bucket>();
                    
                    // Obtener imágenes
                    var imagesResponse = await _httpClient.GetAsync($"api/images/bucket/{id}");
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
    }
}