namespace BusinessObjects.Models;

public class CarBrand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<CarModel> CarModels { get; set; } = [];
    public ICollection<Car> Cars { get; set; } = [];
}
