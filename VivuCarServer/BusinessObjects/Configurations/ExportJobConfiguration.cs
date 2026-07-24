using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class ExportJobConfiguration : IEntityTypeConfiguration<ExportJob>
{
    public void Configure(EntityTypeBuilder<ExportJob> entity)
    {
        entity.ToTable("ExportJobs");
        entity.HasIndex(job => job.RequestedBy);
        entity.HasIndex(job => job.Status);
        entity.Property(job => job.ExportType).HasMaxLength(30).IsRequired();
        entity.Property(job => job.ParamsJson).HasColumnType("nvarchar(max)").IsRequired();
        entity.Property(job => job.Status).HasMaxLength(20).IsRequired();
        entity.Property(job => job.FileUrl).HasMaxLength(500);
        entity.Property(job => job.ErrorMessage).HasMaxLength(2000);
        entity.Property(job => job.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        entity.HasOne(job => job.Requester).WithMany().HasForeignKey(job => job.RequestedBy).OnDelete(DeleteBehavior.Restrict);
    }
}
