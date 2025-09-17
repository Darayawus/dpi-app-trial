using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Localization;

namespace PrintBucket.Web.Pages
{
    public class AccessModel : PageModel
    {
        private readonly ILogger<AccessModel> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccessModel(
            ILogger<AccessModel> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
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

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _logger.LogInformation("Access attempt with code: {Code}", Input.AccessCode);
            return RedirectToPage("/Index");
        }
    }
}