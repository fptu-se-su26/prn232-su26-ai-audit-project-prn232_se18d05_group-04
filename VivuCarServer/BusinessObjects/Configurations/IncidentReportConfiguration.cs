using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class IncidentReportConfiguration : IEntityTypeConfiguration<IncidentReport>
{
    public void Configure(EntityTypeBuilder<IncidentReport> entity)
    {
        entity.ToTable("IncidentReports");
        entity.HasIndex(report => report.BookingId);
        entity.HasIndex(report => report.ReporterId);

        entity.Property(report => report.Description).HasMaxLength(2000).IsRequired();
        entity
            .Property(report => report.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        entity.Property(report => report.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(report => report.Booking)
            .WithMany(booking => booking.IncidentReports)
            .HasForeignKey(report => report.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        entity
            .HasOne(report => report.Reporter)
            .WithMany(user => user.IncidentReports)
            .HasForeignKey(report => report.ReporterId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
