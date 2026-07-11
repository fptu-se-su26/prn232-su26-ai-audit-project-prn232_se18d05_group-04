using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedMiotoCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.RenameColumn(
                name: "DriverLicenseImageUrl",
                table: "DriverDocuments",
                newName: "DriverLicenseFrontImageUrl");

            migrationBuilder.RenameColumn(
                name: "DriverLicenseImageUrl",
                table: "BookingDriverInfos",
                newName: "DriverLicenseFrontImageUrl");

            migrationBuilder.AddColumn<int>(
                name: "TokenVersion",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseBackImageUrl",
                table: "DriverDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

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

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "CarAvailabilityBlocks",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseBackImageUrl",
                table: "BookingDriverInfos",
                type: "nvarchar(500)",
                maxLength: 500,
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

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByTokenHash = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CarBrands",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[] { 1, true, "Toyota" });

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
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Address", "CreatedAt", "DateOfBirth", "Email", "FullName", "PasswordHash", "TokenVersion" },
                values: new object[] { null, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "seed_owner@vivucar.local", "Seed Car Owner", "dummy_hash_for_seed", 1 });

            migrationBuilder.InsertData(
                table: "Vouchers",
                columns: new[] { "Id", "Code", "Description", "DiscountType", "DiscountValue", "EndDateTime", "IsActive", "MaxDiscountAmount", "MinOrderAmount", "StartDateTime", "UsageLimit", "UsedCount" },
                values: new object[] { 1, "VIVUCAR10", "Giảm giá 10% tổng hóa đơn", "Percentage", 10m, new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, 500000m, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 100, 0 });

            migrationBuilder.InsertData(
                table: "CarModels",
                columns: new[] { "Id", "CarBrandId", "IsActive", "Name" },
                values: new object[] { 1, 1, true, "Vios" });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "BlockedReason", "CarBrandId", "CarModelId", "CarTypeId", "Color", "CreatedAt", "DailyPrice", "DeliveryFee", "DepositAmount", "Description", "FuelType", "InsuranceFeePerDay", "KilometersDriven", "LicensePlate", "Location", "Name", "OwnerId", "PreviousStatus", "PricePerHour", "SeatCount", "Status", "TransmissionType", "UpdatedAt", "Year" },
                values: new object[] { 1, null, 1, 1, 1, "White", new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 600000m, 10000m, 1500000m, "Clean 5-seat family car with stable handling and efficient fuel usage.", "Gasoline", 50000m, 28000, "43A-12345", "Hai Chau, Da Nang", "Toyota Vios 2022", 6, null, 90000m, 5, "Available", "Automatic", null, (short)2022 });

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

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_ExpiresAt",
                table: "RefreshTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_TokenHash",
                table: "RefreshTokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

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

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarTypeId",
                table: "Cars");

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarBrands",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "TokenVersion",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DriverLicenseBackImageUrl",
                table: "DriverDocuments");

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

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "CarAvailabilityBlocks");

            migrationBuilder.DropColumn(
                name: "DriverLicenseBackImageUrl",
                table: "BookingDriverInfos");

            migrationBuilder.RenameColumn(
                name: "DriverLicenseFrontImageUrl",
                table: "DriverDocuments",
                newName: "DriverLicenseImageUrl");

            migrationBuilder.RenameColumn(
                name: "DriverLicenseFrontImageUrl",
                table: "BookingDriverInfos",
                newName: "DriverLicenseImageUrl");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Address", "CreatedAt", "DateOfBirth", "Email", "FullName", "PasswordHash" },
                values: new object[] { "District 7, Ho Chi Minh City", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1988, 4, 9), "owner01@vivucar.local", "Vo Quoc Khanh", "SeedPasswordHash_Owner123" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "AvatarUrl", "CreatedAt", "DateOfBirth", "Email", "FullName", "PasswordHash", "PhoneNumber", "Role", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "Ho Chi Minh City", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1990, 1, 1), "admin@vivucar.local", "System Admin", "SeedPasswordHash_Admin123", "0900000001", "Admin", "Active", null },
                    { 2, "District 1, Ho Chi Minh City", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1998, 3, 12), "customer01@vivucar.local", "Nguyen Van An", "SeedPasswordHash_Customer123", "0900000002", "Customer", "Active", null },
                    { 3, "Thu Duc City, Ho Chi Minh City", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1997, 7, 21), "customer02@vivucar.local", "Tran Thi Binh", "SeedPasswordHash_Customer123", "0900000003", "Customer", "Active", null },
                    { 4, "Da Nang", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1995, 11, 5), "customer03@vivucar.local", "Le Minh Chau", "SeedPasswordHash_Customer123", "0900000004", "Customer", "Active", null },
                    { 5, "Can Tho", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1996, 9, 18), "customer04@vivucar.local", "Pham Gia Huy", "SeedPasswordHash_Customer123", "0900000005", "Customer", "Locked", null },
                    { 7, "Binh Thanh, Ho Chi Minh City", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1985, 12, 2), "owner02@vivucar.local", "Dang Hoang Long", "SeedPasswordHash_Owner123", "0900000007", "CarOwner", "Active", null },
                    { 8, "Nha Trang", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1992, 6, 30), "owner03@vivucar.local", "Hoang Bao Tram", "SeedPasswordHash_Owner123", "0900000008", "CarOwner", "Active", null },
                    { 9, "Ha Noi", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1987, 8, 14), "owner04@vivucar.local", "Bui Thanh Son", "SeedPasswordHash_Owner123", "0900000009", "CarOwner", "Locked", null },
                    { 10, "Ho Chi Minh City", null, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(1991, 10, 25), "admin02@vivucar.local", "Support Admin", "SeedPasswordHash_Admin123", "0900000010", "Admin", "Active", null }
                });
        }
    }
}
