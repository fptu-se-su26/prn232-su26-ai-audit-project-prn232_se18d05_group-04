using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

[Table("chat_sessions")]
public class ChatSession
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_id")]
    public int? UserId { get; set; }

    [ForeignKey("UserId")]
    public AppUser? User { get; set; }

    [Column("booking_id")]
    public int? BookingId { get; set; }

    [ForeignKey("BookingId")]
    public Booking? Booking { get; set; }

    [Column("session_type")]
    [MaxLength(20)]
    public string SessionType { get; set; } = "ai"; // ai, live

    [Column("status")]
    [MaxLength(20)]
    public string Status { get; set; } = "open"; // open, escalated, closed

    [Column("assigned_to")]
    public int? AssignedTo { get; set; }

    [ForeignKey("AssignedTo")]
    public AppUser? AssignedOwner { get; set; }

    [Column("escalated_at")]
    public DateTimeOffset? EscalatedAt { get; set; }

    [Column("closed_at")]
    public DateTimeOffset? ClosedAt { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}
