using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddPenaltyAndOverdueFee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'IncidentReports' AND COLUMN_NAME = 'PenaltyAmount'
                )
                BEGIN
                    ALTER TABLE [IncidentReports] ADD [PenaltyAmount] decimal(18,2) NULL
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'IncidentReports' AND COLUMN_NAME = 'Title'
                )
                BEGIN
                    ALTER TABLE [IncidentReports] ADD [Title] nvarchar(max) NOT NULL DEFAULT N''
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'OverdueFee'
                )
                BEGIN
                    ALTER TABLE [Bookings] ADD [OverdueFee] decimal(18,2) NULL
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'IncidentReports' AND COLUMN_NAME = 'PenaltyAmount')
                    ALTER TABLE [IncidentReports] DROP COLUMN [PenaltyAmount];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'IncidentReports' AND COLUMN_NAME = 'Title')
                    ALTER TABLE [IncidentReports] DROP COLUMN [Title];
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Bookings' AND COLUMN_NAME = 'OverdueFee')
                    ALTER TABLE [Bookings] DROP COLUMN [OverdueFee];
            ");
        }
    }
}
