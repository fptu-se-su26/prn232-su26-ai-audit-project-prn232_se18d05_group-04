using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Admin.Cars;

public class FormModel : PageModel
{
    public int? Id { get; private set; }

    public void OnGet(int? id)
    {
        Id = id;
    }
}
