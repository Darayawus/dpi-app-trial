using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace PrintBucket.Web.Pages
{
    public class AccessModel : PageModel
    {
        private readonly ILogger<AccessModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccessModel(
            ILogger<AccessModel> logger,
            IHttpClientFactory httpClientFactory,
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("PrintBucketApi");
            _localizer = localizer;
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
                    if (bucket == null)
                    {
                        ModelState.AddModelError(string.Empty, _localizer["AccessCode_Invalid"]);
                        return Page();
                    }
                    return RedirectToPage("/Bucket", new { id = bucket.Id });
                }
                
                // Manejar específicamente NotFound
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    ModelState.AddModelError(string.Empty, _localizer["AccessCode_Invalid"]);
                    return Page();
                }
                
                // Otros errores HTTP
                ModelState.AddModelError(string.Empty, _localizer["AccessCode_Error"]);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating access code {Code}", Input.AccessCode);
                ModelState.AddModelError(string.Empty, _localizer["AccessCode_Error"]);
                return Page();
            }
        }
    }
}