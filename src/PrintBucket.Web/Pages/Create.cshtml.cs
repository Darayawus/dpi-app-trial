using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;
using PrintBucket.Models;

namespace PrintBucket.Web.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CreateModel(
            ILogger<CreateModel> logger,
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
            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Name is required")]
            [StringLength(100)]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "You must accept the terms")]
            public bool AcceptTerms { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                _logger.LogInformation("Creating bucket for {Email}", Input.Email);

                var request = new
                {
                    name = Input.Name,
                    email = Input.Email,
                    status = "Active"
                };

                var response = await _httpClient.PostAsJsonAsync("api/buckets", request);
                
                if (response.IsSuccessStatusCode)
                {
                    var bucket = await response.Content.ReadFromJsonAsync<Bucket>();
                    if (bucket != null)
                    {
                        _logger.LogInformation("Bucket created successfully with ID {Id}", bucket.Id);
                        // Redirigir al bucket recién creado
                        return RedirectToPage("/Bucket", new { id = bucket.Id });
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string[]>>();
                    if (error != null)
                    {
                        foreach (var item in error)
                        {
                            ModelState.AddModelError(item.Key, item.Value.First());
                        }
                    }
                    return Page();
                }

                ModelState.AddModelError(string.Empty, _localizer["CreateBucket_Error"]);
                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bucket for {Email}", Input.Email);
                ModelState.AddModelError(string.Empty, _localizer["CreateBucket_Error"]);
                return Page();
            }
        }
    }
}