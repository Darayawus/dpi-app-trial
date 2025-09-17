using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using PrintBucket.Web.Resources;
using System.Globalization;

namespace PrintBucket.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IStringLocalizer<SharedResource> localizer,
            ILogger<IndexModel> logger)
        {
            _localizer = localizer;
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}
