using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarTypeConfiguration : IEntityTypeConfiguration<CarType>
{
    public void Configure(EntityTypeBuilder<CarType> entity)
    {
        entity.ToTable("CarTypes");
        entity.HasIndex(type => type.Name).IsUnique();
        entity.Property(type => type.Name).HasMaxLength(50).IsRequired();
        entity.Property(type => type.IsActive).HasDefaultValue(true);

        entity.HasData(
            new CarType { Id = 1, Name = "Sedan", IsActive = true },
            new CarType { Id = 2, Name = "SUV", IsActive = true },
            new CarType { Id = 3, Name = "Hatchback", IsActive = true },
            new CarType { Id = 4, Name = "MPV", IsActive = true }
        );
    }
}
