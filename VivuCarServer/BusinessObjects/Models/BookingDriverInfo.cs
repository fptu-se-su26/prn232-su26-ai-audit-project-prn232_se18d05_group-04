namespace BusinessObjects.Models;

public class BookingDriverInfo
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string CitizenIdNumber { get; set; } = string.Empty;
    public string? CitizenIdFrontImageUrl { get; set; }
    public string? CitizenIdBackImageUrl { get; set; }
    public string DriverLicenseNumber { get; set; } = string.Empty;
    public string? DriverLicenseFrontImageUrl { get; set; }
    public string? DriverLicenseBackImageUrl { get; set; }

    public Booking Booking { get; set; } = null!;
}
