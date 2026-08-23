using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebClient.Pages;

public class RegisterModel(ILogger<RegisterModel> logger) : PageModel
{
    public void OnGet()
    {
        logger.LogInformation("Register page visited.");
    }
}
