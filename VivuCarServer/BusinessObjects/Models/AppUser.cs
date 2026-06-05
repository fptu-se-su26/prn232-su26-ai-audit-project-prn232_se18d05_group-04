using BusinessObjects.Enums;

namespace BusinessObjects.Models;

public class AppUser
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateOnly? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public DriverDocument? DriverDocument { get; set; }
    public ICollection<Car> Cars { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
    public ICollection<BookingStatusHistory> BookingStatusHistories { get; set; } = [];
    public ICollection<NotificationLog> Notifications { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<IncidentReport> IncidentReports { get; set; } = [];
}
