using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Owner.Cars
{
    public class FormModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }
        
        public void OnGet()
        {
        }
    }
}
