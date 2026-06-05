using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class BookingVoucherConfiguration : IEntityTypeConfiguration<BookingVoucher>
{
    public void Configure(EntityTypeBuilder<BookingVoucher> entity)
    {
        entity.ToTable("BookingVouchers");
        entity.HasIndex(bookingVoucher => bookingVoucher.BookingId).IsUnique();
        entity.Property(bookingVoucher => bookingVoucher.Code).HasMaxLength(50).IsRequired();
        entity.Property(bookingVoucher => bookingVoucher.DiscountAmount).HasPrecision(18, 2);
        entity
            .Property(bookingVoucher => bookingVoucher.AppliedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(bookingVoucher => bookingVoucher.Booking)
            .WithOne(booking => booking.BookingVoucher)
            .HasForeignKey<BookingVoucher>(bookingVoucher => bookingVoucher.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(bookingVoucher => bookingVoucher.Voucher)
            .WithMany(voucher => voucher.BookingVouchers)
            .HasForeignKey(bookingVoucher => bookingVoucher.VoucherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
