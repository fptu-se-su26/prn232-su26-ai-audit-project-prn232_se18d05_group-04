using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable("Users");
        entity.HasIndex(user => user.Email).IsUnique();

        entity.Property(user => user.Email).HasMaxLength(256).IsRequired();
        entity.Property(user => user.PasswordHash).HasMaxLength(512).IsRequired();
        entity.Property(user => user.FullName).HasMaxLength(150).IsRequired();
        entity.Property(user => user.PhoneNumber).HasMaxLength(20).IsRequired();
        entity.Property(user => user.AvatarUrl).HasMaxLength(500);
        entity.Property(user => user.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity.Property(user => user.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        entity.Property(user => user.TokenVersion).HasDefaultValue(1).IsRequired();
        entity.Property(user => user.Address).HasMaxLength(500);
        entity.Property(user => user.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
    }
}
