using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class DailyRevenueSnapshotConfiguration : IEntityTypeConfiguration<DailyRevenueSnapshot>
{
    public void Configure(EntityTypeBuilder<DailyRevenueSnapshot> entity)
    {
        entity.ToTable("DailyRevenueSnapshots");
        entity.HasIndex(snapshot => snapshot.SnapshotDate).IsUnique();
        entity.Property(snapshot => snapshot.SnapshotDate).HasColumnType("date");
        entity.Property(snapshot => snapshot.GrossRevenue).HasPrecision(18, 2);
        entity.Property(snapshot => snapshot.NetRevenue).HasPrecision(18, 2);
        entity.Property(snapshot => snapshot.DepositCollected).HasPrecision(18, 2);
        entity.Property(snapshot => snapshot.GeneratedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
