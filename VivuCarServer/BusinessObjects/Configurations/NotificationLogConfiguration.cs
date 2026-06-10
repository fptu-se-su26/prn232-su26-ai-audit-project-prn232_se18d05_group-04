using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class NotificationLogConfiguration : IEntityTypeConfiguration<NotificationLog>
{
    public void Configure(EntityTypeBuilder<NotificationLog> entity)
    {
        entity.ToTable("NotificationLogs");
        entity.HasIndex(notification => notification.BookingId);
        entity.HasIndex(notification => notification.RecipientUserId);

        entity
            .Property(notification => notification.Channel)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(notification => notification.Subject).HasMaxLength(200).IsRequired();
        entity
            .Property(notification => notification.Content)
            .HasColumnType("nvarchar(max)")
            .IsRequired();
        entity
            .Property(notification => notification.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(notification => notification.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(notification => notification.Booking)
            .WithMany(booking => booking.NotificationLogs)
            .HasForeignKey(notification => notification.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(notification => notification.RecipientUser)
            .WithMany(user => user.Notifications)
            .HasForeignKey(notification => notification.RecipientUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
