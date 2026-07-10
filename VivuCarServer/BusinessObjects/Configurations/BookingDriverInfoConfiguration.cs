using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class BookingDriverInfoConfiguration : IEntityTypeConfiguration<BookingDriverInfo>
{
    public void Configure(EntityTypeBuilder<BookingDriverInfo> entity)
    {
        entity.ToTable("BookingDriverInfos");
        entity.HasIndex(info => info.BookingId).IsUnique();
        entity.Property(info => info.FullName).HasMaxLength(150).IsRequired();
        entity.Property(info => info.PhoneNumber).HasMaxLength(20).IsRequired();
        entity.Property(info => info.CitizenIdNumber).HasMaxLength(30).IsRequired();
        entity.Property(info => info.CitizenIdFrontImageUrl).HasMaxLength(500);
        entity.Property(info => info.CitizenIdBackImageUrl).HasMaxLength(500);
        entity.Property(info => info.DriverLicenseNumber).HasMaxLength(30).IsRequired();
        entity.Property(info => info.DriverLicenseFrontImageUrl).HasMaxLength(500);
        entity.Property(info => info.DriverLicenseBackImageUrl).HasMaxLength(500);

        entity
            .HasOne(info => info.Booking)
            .WithOne(booking => booking.DriverInfo)
            .HasForeignKey<BookingDriverInfo>(info => info.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
