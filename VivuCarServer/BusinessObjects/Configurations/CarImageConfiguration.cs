using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarImageConfiguration : IEntityTypeConfiguration<CarImage>
{
    public void Configure(EntityTypeBuilder<CarImage> entity)
    {
        entity.ToTable("CarImages");
        entity.HasIndex(image => new { image.CarId, image.DisplayOrder });
        entity.Property(image => image.ImageUrl).HasMaxLength(500).IsRequired();

        entity
            .HasOne(image => image.Car)
            .WithMany(car => car.Images)
            .HasForeignKey(image => image.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
