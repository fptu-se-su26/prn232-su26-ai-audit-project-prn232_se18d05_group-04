using BusinessObjects.Models;
using BusinessObjects.Enums;
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
        entity.HasIndex(car => car.CarTypeId);
        entity.HasIndex(car => car.Status);

        entity.Property(car => car.Name).HasMaxLength(150).IsRequired();
        entity.Property(car => car.LicensePlate).HasMaxLength(20).IsRequired();
        entity.Property(car => car.Year);
        entity.Property(car => car.Color).HasMaxLength(50);
        entity.Property(car => car.KilometersDriven).HasDefaultValue(0);
        entity.Property(car => car.Description).HasMaxLength(2000);
        entity.Property(car => car.Location).HasMaxLength(300).IsRequired();
        entity.Property(car => car.DailyPrice).HasPrecision(18, 2);
        entity.Property(car => car.PricePerHour).HasPrecision(18, 2).HasDefaultValue(0m);
        entity.Property(car => car.InsuranceFeePerDay).HasPrecision(18, 2);
        entity.Property(car => car.DeliveryFee).HasPrecision(18, 2);
        entity.Property(car => car.DepositAmount).HasPrecision(18, 2);
        entity.Property(car => car.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity.Property(car => car.PreviousStatus).HasConversion<string>().HasMaxLength(32);
        entity.Property(car => car.BlockedReason).HasMaxLength(500);
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

        entity
            .HasOne(car => car.CarType)
            .WithMany(type => type.Cars)
            .HasForeignKey(car => car.CarTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasData(new Car
        {
            Id = 1,
            OwnerId = 6,
            CarBrandId = 1,
            CarModelId = 1,
            CarTypeId = 1,
            Name = "Toyota Vios 2022",
            LicensePlate = "43A-12345",
            Year = 2022,
            Color = "White",
            KilometersDriven = 28000,
            Description = "Clean 5-seat family car with stable handling and efficient fuel usage.",
            Location = "Hai Chau, Da Nang",
            DailyPrice = 600000m,
            PricePerHour = 90000m,
            InsuranceFeePerDay = 50000m,
            DeliveryFee = 10000m,
            DepositAmount = 1500000m,
            Status = CarStatus.Available,
            PreviousStatus = null,
            BlockedReason = null,
            SeatCount = 5,
            TransmissionType = TransmissionType.Automatic,
            FuelType = FuelType.Gasoline,
            CreatedAt = new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}
