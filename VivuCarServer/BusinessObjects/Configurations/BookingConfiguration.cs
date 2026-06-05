using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> entity)
    {
        entity.ToTable("Bookings");
        entity.HasIndex(booking => booking.BookingCode).IsUnique();
        entity.HasIndex(booking => booking.CustomerId);
        entity.HasIndex(booking => booking.CarId);
        entity.HasIndex(booking => booking.Status);
        entity.HasIndex(booking => new { booking.CarId, booking.StartDateTime, booking.EndDateTime });

        entity.Property(booking => booking.BookingCode).HasMaxLength(30).IsRequired();
        entity.Property(booking => booking.PickupLocation).HasMaxLength(300).IsRequired();
        entity.Property(booking => booking.ReturnLocation).HasMaxLength(300).IsRequired();
        entity.Property(booking => booking.BasePrice).HasPrecision(18, 2);
        entity.Property(booking => booking.InsuranceFee).HasPrecision(18, 2);
        entity.Property(booking => booking.DeliveryFee).HasPrecision(18, 2);
        entity.Property(booking => booking.DiscountAmount).HasPrecision(18, 2);
        entity.Property(booking => booking.DepositAmount).HasPrecision(18, 2);
        entity.Property(booking => booking.TotalAmount).HasPrecision(18, 2);
        entity.Property(booking => booking.RemainingAmount).HasPrecision(18, 2);
        entity.Property(booking => booking.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity.Property(booking => booking.CancellationReason).HasMaxLength(500);
        entity.Property(booking => booking.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity.HasOne(booking => booking.Customer)
            .WithMany(user => user.Bookings)
            .HasForeignKey(booking => booking.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(booking => booking.Car)
            .WithMany(car => car.Bookings)
            .HasForeignKey(booking => booking.CarId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
