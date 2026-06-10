using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> entity)
    {
        entity.ToTable("Cars");
        entity.HasIndex(car => car.LicensePlate).IsUnique();
        entity.HasIndex(car => car.OwnerId);
        entity.HasIndex(car => car.CarBrandId);
        entity.HasIndex(car => car.CarModelId);
        entity.HasIndex(car => car.Status);

        entity.Property(car => car.Name).HasMaxLength(150).IsRequired();
        entity.Property(car => car.LicensePlate).HasMaxLength(20).IsRequired();
        entity.Property(car => car.Description).HasMaxLength(2000);
        entity.Property(car => car.Location).HasMaxLength(300).IsRequired();
        entity.Property(car => car.DailyPrice).HasPrecision(18, 2);
        entity.Property(car => car.InsuranceFeePerDay).HasPrecision(18, 2);
        entity.Property(car => car.DeliveryFee).HasPrecision(18, 2);
        entity.Property(car => car.DepositAmount).HasPrecision(18, 2);
        entity.Property(car => car.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity
            .Property(car => car.TransmissionType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(car => car.FuelType).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity.Property(car => car.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(car => car.Owner)
            .WithMany(user => user.Cars)
            .HasForeignKey(car => car.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(car => car.CarBrand)
            .WithMany(brand => brand.Cars)
            .HasForeignKey(car => car.CarBrandId)
            .OnDelete(DeleteBehavior.Restrict);

        entity
            .HasOne(car => car.CarModel)
            .WithMany(model => model.Cars)
            .HasForeignKey(car => car.CarModelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
