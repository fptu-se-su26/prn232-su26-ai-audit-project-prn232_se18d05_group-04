using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public class VivuCarDbContext(DbContextOptions<VivuCarDbContext> options) : DbContext(options)
{
    // Add DbSet properties here after domain models are scaffolded.
}
