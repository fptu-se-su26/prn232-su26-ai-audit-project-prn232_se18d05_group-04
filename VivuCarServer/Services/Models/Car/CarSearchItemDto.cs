namespace Services.Models.Car;

public class CarSearchItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string ModelName { get; set; } = string.Empty;
    public decimal DailyPrice { get; set; }
    public string Location { get; set; } = string.Empty;
    public string? PrimaryImageUrl { get; set; }
    public int SeatCount { get; set; }
    public string Transmission { get; set; } = string.Empty; // Automatic, Manual
    public string Fuel { get; set; } = string.Empty;         // Gasoline, Diesel, Electric, Hybrid
    public double AverageRating { get; set; }
    public int TotalBookings { get; set; }
    public string OwnerName { get; set; } = string.Empty;
}
