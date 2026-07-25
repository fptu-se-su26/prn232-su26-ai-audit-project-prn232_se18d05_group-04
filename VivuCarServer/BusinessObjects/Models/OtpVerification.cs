using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models;

[Table("OtpVerifications")]
public class OtpVerification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [MaxLength(6)]
    public string Code { get; set; } = null!;

    public DateTime ExpiredAt { get; set; }

    public bool IsUsed { get; set; }

    public int AttemptCount { get; set; }

    public DateTime CreatedAt { get; set; }

    public AppUser User { get; set; } = null!;
}
