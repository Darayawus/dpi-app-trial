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
            {
                _logger.LogWarning("Invalid ModelState for access attempt");
                return Page();
            }

            try
            {
                _logger.LogInformation("Attempting to validate access code: {AccessCode} {Url}", Input.AccessCode, _httpClient.BaseAddress);
                
                var response = await _httpClient.GetAsync($"buckets/access/{Input.AccessCode}");
                _logger.LogDebug("API response status: {StatusCode} for access code: {AccessCode}", 
                    response.StatusCode, Input.AccessCode);

                if (response.IsSuccessStatusCode)
                {
                    var bucket = await response.Content.ReadFromJsonAsync<PrintBucket.Models.Bucket>();
                    if (bucket == null)
                    {
                        _logger.LogWarning("API returned success but bucket was null for access code: {AccessCode}", 
                            Input.AccessCode);
                        ModelState.AddModelError(string.Empty, _localizer["AccessCode_Invalid"]);
                        return Page();
                    }

                    _logger.LogInformation("Successfully validated access code: {AccessCode} for bucket: {BucketId}", 
                        Input.AccessCode, bucket.Id);
                    return RedirectToPage("/Bucket", new { id = bucket.Id });
                }
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogInformation("Access code not found: {AccessCode}", Input.AccessCode);
                    ModelState.AddModelError(string.Empty, _localizer["AccessCode_Invalid"]);
                    return Page();
                }
                
                _logger.LogError("Unexpected API response: {StatusCode} for access code: {AccessCode}", 
                    response.StatusCode, Input.AccessCode);
                ModelState.AddModelError(string.Empty, _localizer["AccessCode_Error"]);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating access code: {AccessCode}. Error: {Error}", 
                    Input.AccessCode, ex.Message);
                ModelState.AddModelError(string.Empty, _localizer["AccessCode_Error"]);
                return Page();
            }
        }
    }
}