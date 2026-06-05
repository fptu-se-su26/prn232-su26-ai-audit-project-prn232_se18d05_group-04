namespace BusinessObjects.Models;

public class RentalContract
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string ContractNumber { get; set; } = string.Empty;
    public string PdfUrl { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }

    public Booking Booking { get; set; } = null!;
}
