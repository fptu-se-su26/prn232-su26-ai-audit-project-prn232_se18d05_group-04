using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> entity)
    {
        entity.ToTable(
            "Reviews",
            table => table.HasCheckConstraint("CK_Reviews_Rating", "[Rating] BETWEEN 1 AND 5")
        );
        entity.HasIndex(review => review.BookingId).IsUnique();
        entity.HasIndex(review => review.CarId);
        entity.HasIndex(review => review.CustomerId);

        entity.Property(review => review.Comment).HasMaxLength(2000);
        entity.Property(review => review.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(review => review.Booking)
            .WithOne(booking => booking.Review)
            .HasForeignKey<Review>(review => review.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(review => review.Customer)
            .WithMany(user => user.Reviews)
            .HasForeignKey(review => review.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(review => review.Car)
            .WithMany(car => car.Reviews)
            .HasForeignKey(review => review.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
