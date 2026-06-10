using BusinessObjects.Models;
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
    }
}
