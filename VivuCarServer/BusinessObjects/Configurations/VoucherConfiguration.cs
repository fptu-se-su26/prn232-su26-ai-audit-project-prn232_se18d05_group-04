using BusinessObjects.Models;
using BusinessObjects.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> entity)
    {
        entity.ToTable("Vouchers");
        entity.HasIndex(voucher => voucher.Code).IsUnique();

        entity.Property(voucher => voucher.Code).HasMaxLength(50).IsRequired();
        entity.Property(voucher => voucher.Description).HasMaxLength(500);
        entity
            .Property(voucher => voucher.DiscountType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(voucher => voucher.DiscountValue).HasPrecision(18, 2);
        entity.Property(voucher => voucher.MaxDiscountAmount).HasPrecision(18, 2);
        entity.Property(voucher => voucher.MinOrderAmount).HasPrecision(18, 2);

        entity.HasData(new Voucher
        {
            Id = 1,
            Code = "VIVUCAR10",
            Description = "Giảm giá 10% tổng hóa đơn",
            DiscountType = DiscountType.Percentage,
            DiscountValue = 10m,
            MinOrderAmount = 500000m,
            StartDateTime = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc),
            EndDateTime = new DateTime(2026, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            UsageLimit = 100,
            UsedCount = 0,
            IsActive = true
        });
    }
}
