using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class RentalContractConfiguration : IEntityTypeConfiguration<RentalContract>
{
    public void Configure(EntityTypeBuilder<RentalContract> entity)
    {
        entity.ToTable("RentalContracts");
        entity.HasIndex(contract => contract.BookingId).IsUnique();
        entity.HasIndex(contract => contract.ContractNumber).IsUnique();

        entity.Property(contract => contract.ContractNumber).HasMaxLength(50).IsRequired();
        entity.Property(contract => contract.PdfUrl).HasMaxLength(500).IsRequired();
        entity.Property(contract => contract.GeneratedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(contract => contract.Booking)
            .WithOne(booking => booking.RentalContract)
            .HasForeignKey<RentalContract>(contract => contract.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
