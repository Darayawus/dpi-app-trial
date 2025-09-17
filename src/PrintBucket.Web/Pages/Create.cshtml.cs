using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace PrintBucket.Web.Pages
{
    public class CreateModel : PageModel
    {
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(ILogger<CreateModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "El email es requerido")]
            [EmailAddress(ErrorMessage = "El email no es válido")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "El nombre es requerido")]
            [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
            public string Name { get; set; } = string.Empty;

            [Required(ErrorMessage = "Debes aceptar los términos y condiciones")]
            public bool AcceptTerms { get; set; }
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

            // TODO: Implementar la lógica de creación del espacio
            _logger.LogInformation("Nuevo espacio solicitado para: {Email}", Input.Email);

            return RedirectToPage("/Index");
        }
    }
}