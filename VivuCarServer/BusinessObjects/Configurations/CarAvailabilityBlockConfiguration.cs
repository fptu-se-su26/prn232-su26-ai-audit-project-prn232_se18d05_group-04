using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class CarAvailabilityBlockConfiguration : IEntityTypeConfiguration<CarAvailabilityBlock>
{
    public void Configure(EntityTypeBuilder<CarAvailabilityBlock> entity)
    {
        entity.ToTable("CarAvailabilityBlocks");
        entity.HasIndex(block => new
        {
            block.CarId,
            block.StartDateTime,
            block.EndDateTime,
        });
        entity.Property(block => block.Reason).HasMaxLength(300).IsRequired();

        entity
            .HasOne(block => block.Car)
            .WithMany(car => car.AvailabilityBlocks)
            .HasForeignKey(block => block.CarId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(block => block.Booking)
            .WithMany(booking => booking.AvailabilityBlocks)
            .HasForeignKey(block => block.BookingId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
