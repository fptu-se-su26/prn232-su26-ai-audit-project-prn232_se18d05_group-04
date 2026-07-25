using System;
using System.ComponentModel.DataAnnotations;

namespace Services.Models.Review;

public class ReviewCreateRequest
{
    public int BookingId { get; set; }
    
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [MaxLength(2000)]
    public string? Comment { get; set; }
}

public class ReviewUpdateRequest
{
    [Range(1, 5)]
    public int Rating { get; set; }
    
    [MaxLength(2000)]
    public string? Comment { get; set; }
}

public class ReviewDetailResponse
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public int CarId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CustomerId { get; set; }
}
