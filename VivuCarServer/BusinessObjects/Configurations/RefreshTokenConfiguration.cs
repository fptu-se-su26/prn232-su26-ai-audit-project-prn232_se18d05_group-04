using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessObjects.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> entity)
    {
        entity.ToTable("RefreshTokens");
        entity.HasIndex(token => token.TokenHash).IsUnique();
        entity.HasIndex(token => token.UserId);
        entity.HasIndex(token => token.ExpiresAt);

        entity.Property(token => token.TokenHash).HasMaxLength(128).IsRequired();
        entity.Property(token => token.ReplacedByTokenHash).HasMaxLength(128);
        entity.Property(token => token.CreatedByIp).HasMaxLength(64);
        entity.Property(token => token.RevokedByIp).HasMaxLength(64);
        entity.Property(token => token.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        entity.HasOne(token => token.User)
            .WithMany(user => user.RefreshTokens)
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
