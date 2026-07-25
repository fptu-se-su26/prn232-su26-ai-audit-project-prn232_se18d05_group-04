using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

[Table("chat_messages")]
public class ChatMessage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("session_id")]
    public int SessionId { get; set; }

    [ForeignKey("SessionId")]
    public ChatSession Session { get; set; } = null!;

    [Column("sender_id")]
    public int? SenderId { get; set; }

    [ForeignKey("SenderId")]
    public AppUser? Sender { get; set; }

    [Column("role")]
    [MaxLength(20)]
    public string Role { get; set; } = "user"; // user, assistant, system

    [Column("content")]
    public string Content { get; set; } = null!;

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
