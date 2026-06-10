using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class DriverDocumentConfiguration : IEntityTypeConfiguration<DriverDocument>
{
    public void Configure(EntityTypeBuilder<DriverDocument> entity)
    {
        entity.ToTable("DriverDocuments");
        entity.HasIndex(document => document.UserId).IsUnique();
        entity.HasIndex(document => document.CitizenIdNumber).IsUnique();
        entity.HasIndex(document => document.DriverLicenseNumber).IsUnique();

        entity.Property(document => document.CitizenIdNumber).HasMaxLength(30).IsRequired();
        entity.Property(document => document.CitizenIdFrontImageUrl).HasMaxLength(500);
        entity.Property(document => document.CitizenIdBackImageUrl).HasMaxLength(500);
        entity.Property(document => document.DriverLicenseNumber).HasMaxLength(30).IsRequired();
        entity.Property(document => document.DriverLicenseImageUrl).HasMaxLength(500);
        entity
            .Property(document => document.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(document => document.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(document => document.User)
            .WithOne(user => user.DriverDocument)
            .HasForeignKey<DriverDocument>(document => document.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
