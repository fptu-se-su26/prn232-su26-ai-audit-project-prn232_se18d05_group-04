using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class BookingStatusHistoryConfiguration : IEntityTypeConfiguration<BookingStatusHistory>
{
    public void Configure(EntityTypeBuilder<BookingStatusHistory> entity)
    {
        entity.ToTable("BookingStatusHistories");
        entity.HasIndex(history => history.BookingId);
        entity
            .Property(history => history.OldStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity
            .Property(history => history.NewStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(history => history.Note).HasMaxLength(500);
        entity.Property(history => history.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(history => history.Booking)
            .WithMany(booking => booking.StatusHistories)
            .HasForeignKey(history => history.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(history => history.ChangedByUser)
            .WithMany(user => user.BookingStatusHistories)
            .HasForeignKey(history => history.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
