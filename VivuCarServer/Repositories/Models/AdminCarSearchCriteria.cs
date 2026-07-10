namespace Repositories.Models;

public class AdminCarSearchCriteria
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Keyword { get; set; }
    public string? Status { get; set; }
    public int? TypeId { get; set; }
    public string? FuelType { get; set; }
    public string? Transmission { get; set; }
}
