using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarBrandConfiguration : IEntityTypeConfiguration<CarBrand>
{
    public void Configure(EntityTypeBuilder<CarBrand> entity)
    {
        entity.ToTable("CarBrands");
        entity.HasIndex(brand => brand.Name).IsUnique();
        entity.Property(brand => brand.Name).HasMaxLength(100).IsRequired();
    }
}
