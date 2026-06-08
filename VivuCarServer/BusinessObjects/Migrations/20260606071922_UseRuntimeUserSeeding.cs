using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations;

/// <inheritdoc />
public partial class UseRuntimeUserSeeding : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // User records are preserved. Default accounts are now managed at runtime.
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Runtime-seeded users are intentionally not modified during rollback.
    }
}
