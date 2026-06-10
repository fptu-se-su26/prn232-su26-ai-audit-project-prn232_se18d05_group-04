using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarModelConfiguration : IEntityTypeConfiguration<CarModel>
{
    public void Configure(EntityTypeBuilder<CarModel> entity)
    {
        entity.ToTable("CarModels");
        entity.HasIndex(model => new { model.CarBrandId, model.Name }).IsUnique();
        entity.Property(model => model.Name).HasMaxLength(100).IsRequired();

        entity
            .HasOne(model => model.CarBrand)
            .WithMany(brand => brand.CarModels)
            .HasForeignKey(model => model.CarBrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
