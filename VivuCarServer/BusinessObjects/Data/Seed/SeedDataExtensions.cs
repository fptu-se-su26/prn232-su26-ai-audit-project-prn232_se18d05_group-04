using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data.Seed;

public static class SeedDataExtensions
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Models.AppUser>().HasData(UserSeed.Users);
    }
}
