using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class AdminAuditLogConfiguration : IEntityTypeConfiguration<AdminAuditLog>
{
    public void Configure(EntityTypeBuilder<AdminAuditLog> entity)
    {
        entity.ToTable("AdminAuditLogs");
        entity.HasIndex(log => log.AdminUserId);
        entity.HasIndex(log => new { log.EntityType, log.EntityId });
        entity.HasIndex(log => log.Action);
        entity.Property(log => log.Action).HasMaxLength(80).IsRequired();
        entity.Property(log => log.EntityType).HasMaxLength(80).IsRequired();
        entity.Property(log => log.OldValues).HasColumnType("nvarchar(max)");
        entity.Property(log => log.NewValues).HasColumnType("nvarchar(max)");
        entity.Property(log => log.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        entity
            .HasOne(log => log.AdminUser)
            .WithMany()
            .HasForeignKey(log => log.AdminUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
