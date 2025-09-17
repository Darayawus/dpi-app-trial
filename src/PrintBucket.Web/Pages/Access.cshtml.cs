using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PrintBucket.Web.Pages
{
    public class AccessModel : PageModel
    {
        private readonly ILogger<AccessModel> _logger;

        public AccessModel(ILogger<AccessModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "El c�digo de acceso es requerido")]
            [StringLength(50, MinimumLength = 6, ErrorMessage = "El c�digo debe tener entre 6 y 50 caracteres")]
            public string AccessCode { get; set; } = string.Empty;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // TODO: Implementar la l�gica de validaci�n del c�digo de acceso
            _logger.LogInformation("Intento de acceso con c�digo: {Code}", Input.AccessCode);

            return RedirectToPage("/Index");
        }
    }
}