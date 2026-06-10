namespace BusinessObjects.Models;

public class CarAvailabilityBlock
{
    public int Id { get; set; }
    public int CarId { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int? BookingId { get; set; }

    public Car Car { get; set; } = null!;
    public Booking? Booking { get; set; }
}
