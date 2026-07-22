using System.Collections.Generic;

namespace Services.Models.Car;

/// <summary>
/// Filter options loaded dynamically from available cars in DB.
/// Used by the frontend search/filter sidebar.
/// </summary>
public class CarFilterOptionsDto
{
    public IReadOnlyList<string> Brands { get; set; } = [];
    public IReadOnlyList<string> Districts { get; set; } = [];
    public IReadOnlyList<int> SeatCounts { get; set; } = [];
    public IReadOnlyList<string> Transmissions { get; set; } = [];
    public IReadOnlyList<string> FuelTypes { get; set; } = [];
    public decimal MinPrice { get; set; }
    public decimal MaxPrice { get; set; }
}
