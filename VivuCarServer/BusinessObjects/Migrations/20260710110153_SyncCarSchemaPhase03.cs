using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SyncCarSchemaPhase03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockedReason",
                table: "Cars",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CarTypeId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Cars",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KilometersDriven",
                table: "Cars",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PreviousStatus",
                table: "Cars",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                table: "Cars",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<short>(
                name: "Year",
                table: "Cars",
                type: "smallint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AdminAuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminUserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdminAuditLogs_Users_AdminUserId",
                        column: x => x.AdminUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CarTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CarTypes",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Sedan" },
                    { 2, true, "SUV" },
                    { 3, true, "Hatchback" },
                    { 4, true, "MPV" }
                });

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "BlockedReason", "CarTypeId", "Color", "Description", "KilometersDriven", "Location", "PreviousStatus", "PricePerHour", "Year" },
                values: new object[] { null, 1, "White", "Clean 5-seat family car with stable handling and efficient fuel usage.", 28000, "Hai Chau, Da Nang", null, 90000m, (short)2022 });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarTypeId",
                table: "Cars",
                column: "CarTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_Action",
                table: "AdminAuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_AdminUserId",
                table: "AdminAuditLogs",
                column: "AdminUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AdminAuditLogs_EntityType_EntityId",
                table: "AdminAuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_CarTypes_Name",
                table: "CarTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarTypes_CarTypeId",
                table: "Cars",
                column: "CarTypeId",
                principalTable: "CarTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarTypes_CarTypeId",
                table: "Cars");

            migrationBuilder.DropTable(
                name: "AdminAuditLogs");

            migrationBuilder.DropTable(
                name: "CarTypes");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarTypeId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "BlockedReason",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarTypeId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "KilometersDriven",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "PreviousStatus",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "PricePerHour",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Cars");

            migrationBuilder.UpdateData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Location" },
                values: new object[] { "Xe gia đình 5 chỗ sạch sẽ, vận hành êm ái, tiết kiệm nhiên liệu.", "Hải Châu, Đà Nẵng" });
        }
    }
}
