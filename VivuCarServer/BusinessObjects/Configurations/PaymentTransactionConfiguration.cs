using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> entity)
    {
        entity.ToTable("PaymentTransactions");
        entity.HasIndex(payment => payment.TransactionCode).IsUnique();
        entity.HasIndex(payment => payment.BookingId);

        entity
            .Property(payment => payment.PaymentProvider)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(payment => payment.TransactionCode).HasMaxLength(100).IsRequired();
        entity.Property(payment => payment.Amount).HasPrecision(18, 2);
        entity
            .Property(payment => payment.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(payment => payment.PaymentUrl).HasMaxLength(1000);
        entity.Property(payment => payment.RawRequest).HasColumnType("nvarchar(max)");
        entity.Property(payment => payment.RawResponse).HasColumnType("nvarchar(max)");
        entity.Property(payment => payment.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(payment => payment.Booking)
            .WithMany(booking => booking.PaymentTransactions)
            .HasForeignKey(payment => payment.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
