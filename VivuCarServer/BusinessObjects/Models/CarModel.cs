namespace BusinessObjects.Models;

public class CarModel
{
    public int Id { get; set; }
    public int CarBrandId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public CarBrand CarBrand { get; set; } = null!;
    public ICollection<Car> Cars { get; set; } = [];
}
