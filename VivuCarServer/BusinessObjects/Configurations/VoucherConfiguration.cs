using BusinessObjects.Models;using BusinessObjects.Enums;using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace BusinessObjects.Configurations;
public class VoucherConfiguration:IEntityTypeConfiguration<Voucher>
{
 public void Configure(EntityTypeBuilder<Voucher> e){e.ToTable("Vouchers");e.HasIndex(x=>x.Code).IsUnique();e.Property(x=>x.Name).HasMaxLength(100).IsRequired();e.Property(x=>x.Code).HasMaxLength(50).IsRequired();e.Property(x=>x.DiscountType).HasConversion(v=>v==DiscountType.Percentage?"percentage":"fixed",v=>v=="percentage"?DiscountType.Percentage:DiscountType.Fixed).HasMaxLength(20).IsRequired();e.Property(x=>x.DiscountValue).HasPrecision(18,2);e.Property(x=>x.MinOrderAmount).HasPrecision(18,2).HasDefaultValue(0);e.Property(x=>x.MaxDiscount).HasPrecision(18,2);e.Property(x=>x.Quantity).HasDefaultValue(0);e.Property(x=>x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");e.HasData(new Voucher{Id=1,Name="VivuCar 10%",Code="VIVUCAR10",DiscountType=DiscountType.Percentage,DiscountValue=10m,MinOrderAmount=500000m,MaxDiscount=250000m,Quantity=100,ExpiresAt=new DateTime(2026,12,31,23,59,59,DateTimeKind.Utc),CreatedAt=new DateTime(2026,6,1,0,0,0,DateTimeKind.Utc)});}
}


