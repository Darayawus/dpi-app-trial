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
                ModelState.AddModelError("", "No files selected.");
                return Page();
            }

            // Aquí deberías guardar los archivos en S3 o donde corresponda
            foreach (var file in Files)
            {
                using var stream = file.OpenReadStream();
                // TODO: Llama a tu servicio de almacenamiento, por ejemplo:
                // await _imageService.SaveImageAsync(id, file.FileName, stream);
            }

            // Redirige a la misma página para refrescar la lista de imágenes
            return RedirectToPage(new { id });
        }
    }
}