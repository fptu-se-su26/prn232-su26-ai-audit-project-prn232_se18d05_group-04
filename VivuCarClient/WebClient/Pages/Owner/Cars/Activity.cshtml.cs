using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Owner.Cars
{
    public class ActivityModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public IActionResult OnGet()
        {
            if (Id <= 0) return RedirectToPage("/Owner/Cars/Index");
            return Page();
        }
    }
}
