using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PrintBucket.Web.Pages
{
    public class AccessModel : PageModel
    {
        private readonly ILogger<AccessModel> _logger;
        private readonly HttpClient _httpClient;

        public AccessModel(
            ILogger<AccessModel> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("PrintBucketApi");
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Access code is required")]
            [StringLength(50, MinimumLength = 6, 
                ErrorMessage = "Access code must be between {2} and {1} characters")]
            [Display(Name = "Access Code")]
            public string AccessCode { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                var response = await _httpClient.GetAsync($"api/buckets/access/{Input.AccessCode}");
                if (response.IsSuccessStatusCode)
                {
                    var bucket = await response.Content.ReadFromJsonAsync<PrintBucket.Models.Bucket>();
                    return RedirectToPage("/Bucket", new { id = bucket?.Id });
                }
                
                ModelState.AddModelError(string.Empty, "Invalid access code");
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating access code");
                ModelState.AddModelError(string.Empty, "An error occurred. Please try again later.");
                return Page();
            }
        }
    }
}