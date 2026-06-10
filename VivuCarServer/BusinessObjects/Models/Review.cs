namespace BusinessObjects.Models;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public int CarId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public Booking Booking { get; set; } = null!;
    public AppUser Customer { get; set; } = null!;
    public Car Car { get; set; } = null!;
}
