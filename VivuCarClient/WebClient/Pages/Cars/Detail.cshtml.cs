using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Cars
{
    public class DetailModel : PageModel
    {
        public int CarId { get; set; }

        public void OnGet(int id)
        {
            CarId = id;
        }
    }
}
