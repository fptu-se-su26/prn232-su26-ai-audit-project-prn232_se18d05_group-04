using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Owner.Bookings
{
    public class ReturnModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int Id { get; set; }

        public IActionResult OnGet()
        {
            if (Id <= 0) return RedirectToPage("/Owner/Bookings/Index");
            return Page();
        }
    }
}
