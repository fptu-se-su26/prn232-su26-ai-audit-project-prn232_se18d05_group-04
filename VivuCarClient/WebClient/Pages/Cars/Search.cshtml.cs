using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebClient.Pages.Cars
{
    public class SearchModel : PageModel
    {
        public string? SearchTerm { get; set; }
        public string? Location { get; set; }
        public string? SortBy { get; set; }
        public int? SeatCount { get; set; }

        public void OnGet(string? searchTerm, string? location, string? sortBy, int? seatCount)
        {
            SearchTerm = searchTerm;
            Location = location;
            SortBy = sortBy;
            SeatCount = seatCount;
        }
    }
}
