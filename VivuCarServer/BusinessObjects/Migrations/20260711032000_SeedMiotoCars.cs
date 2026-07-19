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

            migrationBuilder.InsertData(
                table: "CarBrands",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 2, true, "Chevrolet" },
                    { 3, true, "Ford" },
                    { 4, true, "Geely" },
                    { 5, true, "Honda" },
                    { 6, true, "Hyundai" },
                    { 7, true, "Isuzu" },
                    { 8, true, "Kia" },
                    { 9, true, "Mazda" },
                    { 10, true, "Mercedes" },
                    { 11, true, "Mg" },
                    { 12, true, "Mg5" },
                    { 13, true, "Mitsubishi" },
                    { 14, true, "Nissan" },
                    { 15, true, "Omoda" },
                    { 16, true, "Peugeot" },
                    { 17, true, "Suzuki" },
                    { 18, true, "Vinfast" },
                });

            migrationBuilder.InsertData(
                table: "CarModels",
                columns: new[] { "Id", "CarBrandId", "IsActive", "Name" },
                values: new object[,]
                {
                    { 2, 2, true, "Captiva" },
                    { 3, 2, true, "Colorado 4X2" },
                    { 4, 2, true, "Cruze" },
                    { 5, 2, true, "Spark" },
                    { 6, 3, true, "Focus" },
                    { 7, 3, true, "Ranger Xls 4X4" },
                    { 8, 3, true, "Territory Titanium" },
                    { 9, 4, true, "Coolray Flagship" },
                    { 10, 4, true, "Coolray Standard" },
                    { 11, 5, true, "City" },
                    { 12, 5, true, "City Rs" },
                    { 13, 5, true, "Crv G" },
                    { 14, 5, true, "Crv L" },
                    { 15, 5, true, "Crv L Awd" },
                    { 16, 5, true, "Hrv G" },
                    { 17, 5, true, "Jazz" },
                    { 18, 6, true, "Accent" },
                    { 19, 6, true, "Avante" },
                    { 20, 6, true, "Creta" },
                    { 21, 6, true, "Creta Luxury" },
                    { 22, 6, true, "Elantra" },
                    { 23, 6, true, "I10 Sedan" },
                    { 24, 6, true, "Santafe" },
                    { 25, 6, true, "Stargazer" },
                    { 26, 6, true, "Stargazer Premium" },
                    { 27, 6, true, "Tucson Premium" },
                    { 28, 6, true, "Veloser Hatchback" },
                    { 29, 6, true, "Venue" },
                    { 30, 7, true, "Dmax 4X2" },
                    { 31, 8, true, "Carens" },
                    { 32, 8, true, "Carens Luxury" },
                    { 33, 8, true, "Carens Premium" },
                    { 34, 8, true, "Carnival Premium" },
                    { 35, 8, true, "Cerato" },
                    { 36, 8, true, "K3 Premium" },
                    { 37, 8, true, "Morning" },
                    { 38, 8, true, "Sedona Premium" },
                    { 39, 8, true, "Seltos Luxury" },
                    { 40, 8, true, "Seltos Premium" },
                    { 41, 8, true, "Soluto" },
                    { 42, 8, true, "Sonet Luxury" },
                    { 43, 8, true, "Sorento Luxury" },
                    { 44, 8, true, "Sorento Premium" },
                    { 45, 8, true, "Sportage Signature" },
                    { 46, 9, true, "2" },
                    { 47, 9, true, "2 Luxury" },
                    { 48, 9, true, "3 Deluxe" },
                    { 49, 9, true, "3 Luxury" },
                    { 50, 9, true, "3 Premium" },
                    { 51, 9, true, "6 Premium" },
                    { 52, 9, true, "Cx5 Deluxe" },
                    { 53, 9, true, "Cx5 Luxury" },
                    { 54, 9, true, "Cx5 Premium" },
                    { 55, 9, true, "Cx8 Premium" },
                    { 56, 10, true, "C200" },
                    { 57, 10, true, "C200 Exclusive" },
                    { 58, 10, true, "C250" },
                    { 59, 11, true, "Zs Luxury" },
                    { 60, 11, true, "Zs Standard" },
                    { 61, 12, true, "Luxury" },
                    { 62, 12, true, "Mg5" },
                    { 63, 12, true, "Standard" },
                    { 64, 13, true, "Attrage" },
                    { 65, 13, true, "Mirage" },
                    { 66, 13, true, "Outlander" },
                    { 67, 13, true, "Outlander Premium" },
                    { 68, 13, true, "Pajero" },
                    { 69, 13, true, "Xforce Ultimate" },
                    { 70, 13, true, "Xpander" },
                    { 71, 14, true, "Almera El" },
                    { 72, 14, true, "Almera Vl" },
                    { 73, 14, true, "Navara" },
                    { 74, 14, true, "Navara 4X2" },
                    { 75, 15, true, "C5 Flagship Turbo" },
                    { 76, 16, true, "3008" },
                    { 77, 17, true, "Ciaz" },
                    { 78, 17, true, "Ertiga" },
                    { 79, 17, true, "Swift Hatchback" },
                    { 80, 1, true, "Avanza" },
                    { 81, 1, true, "Corolla Cross V" },
                    { 82, 1, true, "Fortuner" },
                    { 83, 1, true, "Innova" },
                    { 84, 1, true, "Innova Cross" },
                    { 85, 1, true, "Veloz Cross" },
                    { 86, 1, true, "Yaris Cross" },
                    { 87, 18, true, "Limo Green" },
                    { 88, 18, true, "Lux A" },
                    { 89, 18, true, "Lux Sa" },
                    { 90, 18, true, "Minio Green" },
                    { 91, 18, true, "Vf3" },
                    { 92, 18, true, "Vf5" },
                    { 93, 18, true, "Vf6 Eco" },
                    { 94, 18, true, "Vf6 Plus" },
                    { 95, 18, true, "Vf7" },
                    { 96, 18, true, "Vf7 Eco" },
                    { 97, 18, true, "Vf7 Plus" },
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "CarBrandId", "CarModelId", "CreatedAt", "DailyPrice", "DeliveryFee", "DepositAmount", "Description", "FuelType", "InsuranceFeePerDay", "LicensePlate", "Location", "Name", "OwnerId", "SeatCount", "Status", "TransmissionType", "UpdatedAt", "PreviousStatus", "PricePerHour", "Year", "CarTypeId", "KilometersDriven", "Color" },
            values: new object[,]
                {
                    { 2, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 798000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90001", "Phường Hải Châu  I, Quận Hải Châu", "Mitsubishi Xpander 2019", 6, 7, "Available", "Automatic", null, null, 99750m, (short)2019, 4, 15566, "Silver" },
                    { 3, 2, 5, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 412000m, 20000m, 1500000m, "Excellent Chevrolet Spark 2012 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 33000m, "43A-90002", "Phường Nại Hiên Đông, Quận Sơn Trà", "Chevrolet Spark 2012", 6, 5, "Available", "Manual", null, null, 295000m, (short)2012, 1, 15849, "Gray" },
                    { 4, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 481000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 38000m, "43A-90003", "Phường Tam Thuận, Quận Thanh Khê", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 316000m, (short)2025, 3, 16132, "Red" },
                    { 5, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90004", "Phường Nại Hiên Đông, Quận Sơn Trà", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 379000m, (short)2025, 3, 16415, "Blue" },
                    { 6, 12, 61, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 653000m, 20000m, 1500000m, "Excellent Mg5 Luxury 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 52000m, "43A-90005", "Phường Tân Chính, Quận Thanh Khê", "Mg5 Luxury 2024", 6, 5, "Available", "Manual", null, null, 452000m, (short)2024, 1, 16698, "White" },
                    { 7, 8, 36, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 934000m, 20000m, 2500000m, "Excellent Kia K3 Premium 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 75000m, "43A-90006", "Phường Tam Thuận, Quận Thanh Khê", "Kia K3 Premium 2022", 6, 4, "Available", "Automatic", null, null, 116750m, (short)2022, 3, 16981, "Black" },
                    { 8, 1, 84, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1177000m, 20000m, 3000000m, "Excellent Toyota Innova Cross 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 94000m, "43A-90007", "Phường Phước Ninh, Quận Hải Châu", "Toyota Innova Cross 2025", 6, 8, "Available", "Automatic", null, null, 778000m, (short)2025, 4, 17264, "Silver" },
                    { 9, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 565000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 45000m, "43A-90008", "Phường Nại Hiên Đông, Quận Sơn Trà", "Vinfast Vf3 2026", 6, 4, "Available", "Automatic", null, null, 371000m, (short)2026, 3, 17547, "Gray" },
                    { 10, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 589000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 47000m, "43A-90009", "Phường Nại Hiên Đông, Quận Sơn Trà", "Mitsubishi Attrage 2022", 6, 4, "Available", "Automatic", null, null, 73625m, (short)2022, 3, 17830, "Red" },
                    { 11, 2, 4, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 577000m, 20000m, 1500000m, "Excellent Chevrolet Cruze 2015 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 46000m, "43A-90010", "Phường Hải Châu  I, Quận Hải Châu", "Chevrolet Cruze 2015", 6, 5, "Available", "Automatic", null, null, 406000m, (short)2015, 1, 18113, "Blue" },
                    { 12, 18, 89, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1028000m, 20000m, 2500000m, "Excellent Vinfast Lux Sa 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 82000m, "43A-90011", "Phường An Hải Bắc, Quận Sơn Trà", "Vinfast Lux Sa 2021", 6, 7, "Available", "Automatic", null, null, 128500m, (short)2021, 4, 18396, "White" },
                    { 13, 18, 96, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1298000m, 20000m, 3000000m, "Excellent Vinfast Vf7 Eco 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 104000m, "43A-90012", "Phường Hải Châu  I, Quận Hải Châu", "Vinfast Vf7 Eco 2026", 6, 4, "Available", "Automatic", null, null, 827000m, (short)2026, 3, 18679, "Black" },
                    { 14, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1079000m, 20000m, 2500000m, "Excellent Vinfast Limo Green 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 86000m, "43A-90013", "Phường Thạch Thang, Quận Hải Châu", "Vinfast Limo Green 2025", 6, 7, "Available", "Automatic", null, null, 696000m, (short)2025, 4, 18962, "Silver" },
                    { 15, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 545000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 44000m, "43A-90014", "Phường Thanh Bình, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 358000m, (short)2025, 3, 19245, "Gray" },
                    { 16, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 807000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 65000m, "43A-90015", "Phường Thạch Thang, Quận Hải Châu", "Vinfast Vf5 2024", 6, 4, "Available", "Automatic", null, null, 100875m, (short)2024, 3, 19528, "Red" },
                    { 17, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 979000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 78000m, "43A-90016", "Phường An Hải Bắc, Quận Sơn Trà", "Mitsubishi Xpander 2022", 6, 7, "Available", "Automatic", null, null, 659000m, (short)2022, 4, 19811, "Blue" },
                    { 18, 8, 41, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 566000m, 20000m, 1500000m, "Excellent Kia Soluto 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 45000m, "43A-90017", "Phường Nại Hiên Đông, Quận Sơn Trà", "Kia Soluto 2020", 6, 5, "Available", "Automatic", null, null, 400000m, (short)2020, 1, 20094, "White" },
                    { 19, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1309000m, 20000m, 3500000m, "Excellent Vinfast Limo Green 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 105000m, "43A-90018", "Phường Tam Thuận, Quận Thanh Khê", "Vinfast Limo Green 2026", 6, 7, "Available", "Automatic", null, null, 833000m, (short)2026, 4, 20377, "Black" },
                    { 20, 1, 83, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 768000m, 20000m, 2000000m, "Excellent Toyota Innova 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 61000m, "43A-90019", "Phường An Hải Bắc, Quận Sơn Trà", "Toyota Innova 2018", 6, 8, "Available", "Manual", null, null, 551000m, (short)2018, 4, 20660, "Silver" },
                    { 21, 2, 2, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 876000m, 20000m, 2000000m, "Excellent Chevrolet Captiva 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90020", "Phường An Hải Bắc, Quận Sơn Trà", "Chevrolet Captiva 2016", 6, 7, "Available", "Automatic", null, null, 109500m, (short)2016, 4, 20943, "Gray" },
                    { 22, 6, 25, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1015000m, 20000m, 2500000m, "Excellent Hyundai Stargazer 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 81000m, "43A-90021", "Phường Thanh Bình, Quận Hải Châu", "Hyundai Stargazer 2024", 6, 7, "Available", "Automatic", null, null, 681000m, (short)2024, 4, 21226, "Red" },
                    { 23, 8, 35, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 702000m, 20000m, 2000000m, "Excellent Kia Cerato 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 56000m, "43A-90022", "Phường An Hải Tây, Quận Sơn Trà", "Kia Cerato 2019", 6, 5, "Available", "Automatic", null, null, 481000m, (short)2019, 1, 21509, "Blue" },
                    { 24, 9, 49, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 807000m, 20000m, 2000000m, "Excellent Mazda 3 Luxury 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90023", "Phường Tân Chính, Quận Thanh Khê", "Mazda 3 Luxury 2020", 6, 4, "Available", "Automatic", null, null, 544000m, (short)2020, 3, 21792, "White" },
                    { 25, 2, 3, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 895000m, 20000m, 2000000m, "Excellent Chevrolet Colorado 4X2 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90024", "Phường An Hải Đông, Quận Sơn Trà", "Chevrolet Colorado 4X2 2018", 6, 4, "Available", "Manual", null, null, 537000m, (short)2018, 3, 22075, "Black" },
                    { 26, 11, 60, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 917000m, 20000m, 2500000m, "Excellent Mg Zs Standard 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90025", "Phường Hải Châu II, Quận Hải Châu", "Mg Zs Standard 2024", 6, 5, "Available", "Automatic", null, null, 550000m, (short)2024, 1, 22358, "Silver" },
                    { 27, 15, 75, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1033000m, 20000m, 2500000m, "Excellent Omoda C5 Flagship Turbo 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 83000m, "43A-90026", "Phường Tam Thuận, Quận Thanh Khê", "Omoda C5 Flagship Turbo 2025", 6, 4, "Available", "Automatic", null, null, 620000m, (short)2025, 3, 22641, "Gray" },
                    { 28, 9, 53, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1372000m, 20000m, 3500000m, "Excellent Mazda Cx5 Luxury 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 110000m, "43A-90027", "Phường Thanh Bình, Quận Hải Châu", "Mazda Cx5 Luxury 2024", 6, 5, "Available", "Automatic", null, null, 823000m, (short)2024, 2, 22924, "Red" },
                    { 29, 11, 60, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 884000m, 20000m, 2000000m, "Excellent Mg Zs Standard 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 71000m, "43A-90028", "Phường An Hải Bắc, Quận Sơn Trà", "Mg Zs Standard 2021", 6, 5, "Available", "Automatic", null, null, 110500m, (short)2021, 1, 23207, "Blue" },
                    { 30, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 566000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 45000m, "43A-90029", "Phường An Hải Bắc, Quận Sơn Trà", "Mitsubishi Attrage 2020", 6, 5, "Available", "Automatic", null, null, 400000m, (short)2020, 1, 23490, "White" },
                    { 31, 8, 34, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1938000m, 20000m, 5000000m, "Excellent Kia Carnival Premium 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 155000m, "43A-90030", "Phường Thuận Phước, Quận Hải Châu", "Kia Carnival Premium 2021", 6, 8, "Available", "Automatic", null, null, 242250m, (short)2021, 4, 23773, "Black" },
                    { 32, 1, 82, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 865000m, 20000m, 2000000m, "Excellent Toyota Fortuner 2009 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 69000m, "43A-90031", "Phường Thạch Thang, Quận Hải Châu", "Toyota Fortuner 2009", 6, 7, "Available", "Automatic", null, null, 591000m, (short)2009, 4, 24056, "Silver" },
                    { 33, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1105000m, 20000m, 3000000m, "Excellent Toyota Veloz Cross 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 88000m, "43A-90032", "Phường Hải Châu  I, Quận Hải Châu", "Toyota Veloz Cross 2024", 6, 7, "Available", "Automatic", null, null, 735000m, (short)2024, 4, 24339, "Gray" },
                    { 34, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 715000m, 20000m, 2000000m, "Excellent Toyota Vios 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 57000m, "43A-90033", "Phường Nại Hiên Đông, Quận Sơn Trà", "Toyota Vios 2018", 6, 4, "Available", "Automatic", null, null, 489000m, (short)2018, 3, 24622, "Red" },
                    { 35, 9, 48, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 772000m, 20000m, 2000000m, "Excellent Mazda 3 Deluxe 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 62000m, "43A-90034", "Phường Thạch Thang, Quận Hải Châu", "Mazda 3 Deluxe 2017", 6, 4, "Available", "Automatic", null, null, 96500m, (short)2017, 3, 24905, "Blue" },
                    { 36, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 883000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 71000m, "43A-90035", "Phường Hải Châu II, Quận Hải Châu", "Mitsubishi Xpander 2023", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2023, 4, 25188, "White" },
                    { 37, 13, 66, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 952000m, 20000m, 2500000m, "Excellent Mitsubishi Outlander 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 76000m, "43A-90036", "Phường Thạch Thang, Quận Hải Châu", "Mitsubishi Outlander 2019", 6, 7, "Available", "Automatic", null, null, 661000m, (short)2019, 4, 25471, "Black" },
                    { 38, 12, 63, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 761000m, 20000m, 2000000m, "Excellent Mg5 Standard 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 61000m, "43A-90037", "Phường Thuận Phước, Quận Hải Châu", "Mg5 Standard 2025", 6, 4, "Available", "Automatic", null, null, 95125m, (short)2025, 3, 25754, "Silver" },
                    { 39, 1, 80, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1068000m, 20000m, 2500000m, "Excellent Toyota Avanza 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 85000m, "43A-90038", "Phường Nại Hiên Đông, Quận Sơn Trà", "Toyota Avanza 2023", 6, 7, "Available", "Manual", null, null, 133500m, (short)2023, 4, 26037, "Gray" },
                    { 40, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 531000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 42000m, "43A-90039", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mitsubishi Attrage 2018", 6, 5, "Available", "Automatic", null, null, 66375m, (short)2018, 1, 26320, "Red" },
                    { 41, 9, 46, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 630000m, 20000m, 1500000m, "Excellent Mazda 2 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 50000m, "43A-90040", "Phường Hòa Thuận Đông, Quận Hải Châu", "Mazda 2 2024", 6, 5, "Available", "Automatic", null, null, 438000m, (short)2024, 1, 26603, "Blue" },
                    { 42, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 787000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 63000m, "43A-90041", "Phường Mân Thái, Quận Sơn Trà", "Mitsubishi Xpander 2019", 6, 7, "Available", "Automatic", null, null, 544000m, (short)2019, 4, 26886, "White" },
                    { 43, 6, 28, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 445000m, 20000m, 1500000m, "Excellent Hyundai Veloser Hatchback 2011 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 36000m, "43A-90042", "Phường Xuân Hà, Quận Thanh Khê", "Hyundai Veloser Hatchback 2011", 6, 4, "Available", "Automatic", null, null, 55625m, (short)2011, 3, 27169, "Black" },
                    { 44, 17, 79, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 556000m, 20000m, 1500000m, "Excellent Suzuki Swift Hatchback 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 44000m, "43A-90043", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Suzuki Swift Hatchback 2016", 6, 4, "Available", "Automatic", null, null, 69500m, (short)2016, 3, 27452, "Silver" },
                    { 45, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 734000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 59000m, "43A-90044", "Phường An Hải Bắc, Quận Sơn Trà", "Vinfast Vf5 2024", 6, 5, "Available", "Automatic", null, null, 482000m, (short)2024, 1, 27735, "Gray" },
                    { 46, 5, 12, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 876000m, 20000m, 2000000m, "Excellent Honda City Rs 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90045", "Phường Chính Gián, Quận Thanh Khê", "Honda City Rs 2025", 6, 5, "Available", "Automatic", null, null, 585000m, (short)2025, 1, 28018, "Red" },
                    { 47, 18, 88, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1112000m, 20000m, 3000000m, "Excellent Vinfast Lux A 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 89000m, "43A-90046", "Phường Xuân Hà, Quận Thanh Khê", "Vinfast Lux A 2021", 6, 4, "Available", "Automatic", null, null, 727000m, (short)2021, 3, 28301, "Blue" },
                    { 48, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 658000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 53000m, "43A-90047", "Phường An Hải Tây, Quận Sơn Trà", "Mitsubishi Attrage 2024", 6, 5, "Available", "Automatic", null, null, 455000m, (short)2024, 1, 28584, "White" },
                    { 49, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 527000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90048", "Phường Hòa Cường Bắc, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 346000m, (short)2025, 3, 28867, "Black" },
                    { 50, 6, 24, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1602000m, 20000m, 4000000m, "Excellent Hyundai Santafe 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 128000m, "43A-90049", "Phường Chính Gián, Quận Thanh Khê", "Hyundai Santafe 2024", 6, 7, "Available", "Automatic", null, null, 1033000m, (short)2024, 4, 29150, "Silver" },
                    { 51, 9, 49, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 910000m, 20000m, 2500000m, "Excellent Mazda 3 Luxury 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90050", "Phường Phước Mỹ, Quận Sơn Trà", "Mazda 3 Luxury 2024", 6, 4, "Available", "Automatic", null, null, 606000m, (short)2024, 3, 29433, "Gray" },
                    { 52, 8, 44, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1244000m, 20000m, 3000000m, "Excellent Kia Sorento Premium 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 100000m, "43A-90051", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Kia Sorento Premium 2018", 6, 7, "Available", "Automatic", null, null, 818000m, (short)2018, 4, 29716, "Red" },
                    { 53, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90052", "Phường Hòa Thuận Đông, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2025, 3, 29999, "Blue" },
                    { 54, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 555000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 44000m, "43A-90053", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Vinfast Vf3 2026", 6, 4, "Available", "Automatic", null, null, 365000m, (short)2026, 3, 30282, "White" },
                    { 55, 5, 11, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 589000m, 20000m, 1500000m, "Excellent Honda City 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 47000m, "43A-90054", "Phường An Hải Đông, Quận Sơn Trà", "Honda City 2016", 6, 5, "Available", "Manual", null, null, 413000m, (short)2016, 1, 30565, "Black" },
                    { 56, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 912000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90055", "Phường Hòa Thuận Tây, Quận Hải Châu", "Mitsubishi Xpander 2024", 6, 7, "Available", "Automatic", null, null, 114000m, (short)2024, 4, 30848, "Silver" },
                    { 57, 13, 66, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 971000m, 20000m, 2500000m, "Excellent Mitsubishi Outlander 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 78000m, "43A-90056", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mitsubishi Outlander 2018", 6, 7, "Available", "Automatic", null, null, 654000m, (short)2018, 4, 31131, "Gray" },
                    { 58, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90057", "Phường Hòa Cường Nam, Quận Hải Châu", "Mitsubishi Xpander 2025", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2025, 4, 31414, "Red" },
                    { 59, 6, 18, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 818000m, 20000m, 2000000m, "Excellent Hyundai Accent 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90058", "Phường Xuân Hà, Quận Thanh Khê", "Hyundai Accent 2020", 6, 4, "Available", "Automatic", null, null, 102250m, (short)2020, 3, 31697, "Blue" },
                    { 60, 1, 83, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1004000m, 20000m, 2500000m, "Excellent Toyota Innova 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 80000m, "43A-90059", "Phường Hải Châu  I, Quận Hải Châu", "Toyota Innova 2019", 6, 8, "Available", "Automatic", null, null, 674000m, (short)2019, 4, 31980, "White" },
                    { 61, 8, 41, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 645000m, 20000m, 1500000m, "Excellent Kia Soluto 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 52000m, "43A-90060", "Phường Hòa Khê, Quận Thanh Khê", "Kia Soluto 2021", 6, 4, "Available", "Automatic", null, null, 80625m, (short)2021, 3, 32263, "Black" },
                    { 62, 1, 83, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 728000m, 20000m, 2000000m, "Excellent Toyota Innova 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 58000m, "43A-90061", "Phường Thạc Gián, Quận Thanh Khê", "Toyota Innova 2017", 6, 8, "Available", "Manual", null, null, 509000m, (short)2017, 4, 32546, "Silver" },
                    { 63, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 539000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 43000m, "43A-90062", "Phường Xuân Hà, Quận Thanh Khê", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 67375m, (short)2025, 3, 32829, "Gray" },
                    { 64, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 661000m, 20000m, 1500000m, "Excellent Mitsubishi Xpander 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 53000m, "43A-90063", "Phường Xuân Hà, Quận Thanh Khê", "Mitsubishi Xpander 2020", 6, 7, "Available", "Manual", null, null, 82625m, (short)2020, 4, 33112, "Red" },
                    { 65, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 878000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90064", "Phường Thanh Khê Đông, Quận Thanh Khê", "Mitsubishi Xpander 2019", 6, 7, "Available", "Automatic", null, null, 599000m, (short)2019, 4, 33395, "Blue" },
                    { 66, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 532000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 43000m, "43A-90065", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 350000m, (short)2024, 3, 33678, "White" },
                    { 67, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 787000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 63000m, "43A-90066", "Phường Chính Gián, Quận Thanh Khê", "Mitsubishi Xpander 2023", 6, 7, "Available", "Automatic", null, null, 544000m, (short)2023, 4, 33961, "Black" },
                    { 68, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90067", "Phường Vĩnh Trung, Quận Thanh Khê", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2024, 3, 34244, "Silver" },
                    { 69, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 711000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 57000m, "43A-90068", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mitsubishi Xpander 2022", 6, 7, "Available", "Automatic", null, null, 517000m, (short)2022, 4, 34527, "Gray" },
                    { 70, 18, 93, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 912000m, 20000m, 2500000m, "Excellent Vinfast Vf6 Eco 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 73000m, "43A-90069", "Phường An Hải Tây, Quận Sơn Trà", "Vinfast Vf6 Eco 2024", 6, 5, "Available", "Automatic", null, null, 114000m, (short)2024, 1, 34810, "Red" },
                    { 71, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 849000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 68000m, "43A-90070", "Phường Hòa Khê, Quận Thanh Khê", "Mitsubishi Xpander 2022", 6, 7, "Available", "Automatic", null, null, 599000m, (short)2022, 4, 35093, "Blue" },
                    { 72, 10, 56, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 885000m, 20000m, 2000000m, "Excellent Mercedes C200 2008 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 71000m, "43A-90071", "Phường Mân Thái, Quận Sơn Trà", "Mercedes C200 2008", 6, 5, "Available", "Automatic", null, null, 591000m, (short)2008, 1, 35376, "White" },
                    { 73, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 795000m, 20000m, 2000000m, "Excellent Toyota Vios 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90072", "Phường Hòa Thuận Tây, Quận Hải Châu", "Toyota Vios 2022", 6, 5, "Available", "Automatic", null, null, 99375m, (short)2022, 1, 35659, "Black" },
                    { 74, 8, 39, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 798000m, 20000m, 2000000m, "Excellent Kia Seltos Luxury 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90073", "Phường Chính Gián, Quận Thanh Khê", "Kia Seltos Luxury 2021", 6, 5, "Available", "Automatic", null, null, 551000m, (short)2021, 1, 35942, "Silver" },
                    { 75, 13, 65, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 501000m, 20000m, 1500000m, "Excellent Mitsubishi Mirage 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 40000m, "43A-90074", "Phường An Hải Tây, Quận Sơn Trà", "Mitsubishi Mirage 2018", 6, 5, "Available", "Manual", null, null, 349000m, (short)2018, 1, 36225, "Gray" },
                    { 76, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 873000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90075", "Phường Chính Gián, Quận Thanh Khê", "Mitsubishi Xpander 2020", 6, 7, "Available", "Automatic", null, null, 596000m, (short)2020, 4, 36508, "Red" },
                    { 77, 8, 42, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 971000m, 20000m, 2500000m, "Excellent Kia Sonet Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 78000m, "43A-90076", "Phường Hòa Thuận Đông, Quận Hải Châu", "Kia Sonet Luxury 2025", 6, 5, "Available", "Automatic", null, null, 121375m, (short)2025, 1, 36791, "Blue" },
                    { 78, 13, 66, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1105000m, 20000m, 3000000m, "Excellent Mitsubishi Outlander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 88000m, "43A-90077", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mitsubishi Outlander 2022", 6, 7, "Available", "Automatic", null, null, 138125m, (short)2022, 4, 37074, "White" },
                    { 79, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 763000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 61000m, "43A-90078", "Phường Xuân Hà, Quận Thanh Khê", "Mitsubishi Xpander 2021", 6, 7, "Available", "Automatic", null, null, 530000m, (short)2021, 4, 37357, "Black" },
                    { 80, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 787000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 63000m, "43A-90079", "Phường Chính Gián, Quận Thanh Khê", "Mitsubishi Xpander 2024", 6, 7, "Available", "Automatic", null, null, 544000m, (short)2024, 4, 37640, "Silver" },
                    { 81, 9, 50, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 704000m, 20000m, 2000000m, "Excellent Mazda 3 Premium 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 56000m, "43A-90080", "Phường Chính Gián, Quận Thanh Khê", "Mazda 3 Premium 2017", 6, 4, "Available", "Automatic", null, null, 88000m, (short)2017, 3, 37923, "Gray" },
                    { 82, 8, 43, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 965000m, 20000m, 2500000m, "Excellent Kia Sorento Luxury 2015 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 77000m, "43A-90081", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Kia Sorento Luxury 2015", 6, 7, "Available", "Automatic", null, null, 120625m, (short)2015, 4, 38206, "Red" },
                    { 83, 8, 31, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1200000m, 20000m, 3000000m, "Excellent Kia Carens 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 96000m, "43A-90082", "Phường Bình Hiên, Quận Hải Châu", "Kia Carens 2025", 6, 6, "Available", "Automatic", null, null, 792000m, (short)2025, 1, 38489, "Blue" },
                    { 84, 8, 31, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1168000m, 20000m, 3000000m, "Excellent Kia Carens 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 93000m, "43A-90083", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Kia Carens 2025", 6, 7, "Available", "Automatic", null, null, 773000m, (short)2025, 4, 38772, "White" },
                    { 85, 16, 76, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 861000m, 20000m, 2000000m, "Excellent Peugeot 3008 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 69000m, "43A-90084", "Phường Thạc Gián, Quận Thanh Khê", "Peugeot 3008 2018", 6, 5, "Available", "Automatic", null, null, 517000m, (short)2018, 1, 39055, "Black" },
                    { 86, 6, 21, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1186000m, 20000m, 3000000m, "Excellent Hyundai Creta Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 95000m, "43A-90085", "Phường An Hải Bắc, Quận Sơn Trà", "Hyundai Creta Luxury 2025", 6, 5, "Available", "Automatic", null, null, 712000m, (short)2025, 1, 39338, "Silver" },
                    { 87, 9, 52, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1045000m, 20000m, 2500000m, "Excellent Mazda Cx5 Deluxe 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 84000m, "43A-90086", "Phường Hòa Thuận Tây, Quận Hải Châu", "Mazda Cx5 Deluxe 2018", 6, 5, "Available", "Automatic", null, null, 130625m, (short)2018, 2, 39621, "Gray" },
                    { 88, 3, 8, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1263000m, 20000m, 3000000m, "Excellent Ford Territory Titanium 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 101000m, "43A-90087", "Phường Vĩnh Trung, Quận Thanh Khê", "Ford Territory Titanium 2023", 6, 4, "Available", "Automatic", null, null, 758000m, (short)2023, 3, 39904, "Red" },
                    { 89, 9, 52, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1378000m, 20000m, 3500000m, "Excellent Mazda Cx5 Deluxe 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 110000m, "43A-90088", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Mazda Cx5 Deluxe 2024", 6, 5, "Available", "Automatic", null, null, 172250m, (short)2024, 2, 40187, "Blue" },
                    { 90, 9, 53, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1435000m, 20000m, 3500000m, "Excellent Mazda Cx5 Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 115000m, "43A-90089", "Phường Thanh Khê Tây, Quận Thanh Khê", "Mazda Cx5 Luxury 2025", 6, 5, "Available", "Automatic", null, null, 861000m, (short)2025, 2, 40470, "White" },
                    { 91, 6, 20, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1032000m, 20000m, 2500000m, "Excellent Hyundai Creta 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 83000m, "43A-90090", "Phường Hải Châu  I, Quận Hải Châu", "Hyundai Creta 2022", 6, 5, "Available", "Automatic", null, null, 619000m, (short)2022, 1, 40753, "Black" },
                    { 92, 11, 60, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 925000m, 20000m, 2500000m, "Excellent Mg Zs Standard 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 74000m, "43A-90091", "Phường Hòa Thuận Tây, Quận Hải Châu", "Mg Zs Standard 2024", 6, 5, "Available", "Automatic", null, null, 555000m, (short)2024, 1, 41036, "Silver" },
                    { 93, 8, 33, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1054000m, 20000m, 2500000m, "Excellent Kia Carens Premium 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 84000m, "43A-90092", "Phường Xuân Hà, Quận Thanh Khê", "Kia Carens Premium 2023", 6, 7, "Available", "Automatic", null, null, 705000m, (short)2023, 4, 41319, "Gray" },
                    { 94, 9, 46, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 834000m, 20000m, 2000000m, "Excellent Mazda 2 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 67000m, "43A-90093", "Phường Vĩnh Trung, Quận Thanh Khê", "Mazda 2 2024", 6, 5, "Available", "Automatic", null, null, 561000m, (short)2024, 1, 41602, "Red" },
                    { 95, 5, 11, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 818000m, 20000m, 2000000m, "Excellent Honda City 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90094", "Phường Thọ Quang, Quận Sơn Trà", "Honda City 2024", 6, 5, "Available", "Automatic", null, null, 102250m, (short)2024, 1, 41885, "Blue" },
                    { 96, 9, 55, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1547000m, 20000m, 4000000m, "Excellent Mazda Cx8 Premium 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 124000m, "43A-90095", "Phường Nại Hiên Đông, Quận Sơn Trà", "Mazda Cx8 Premium 2023", 6, 6, "Available", "Automatic", null, null, 193375m, (short)2023, 2, 42168, "White" },
                    { 97, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 838000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 67000m, "43A-90096", "Phường Hòa Khê, Quận Thanh Khê", "Vinfast Vf5 2025", 6, 4, "Available", "Automatic", null, null, 551000m, (short)2025, 3, 42451, "Black" },
                    { 98, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1125000m, 20000m, 3000000m, "Excellent Vinfast Limo Green 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 90000m, "43A-90097", "Phường Nại Hiên Đông, Quận Sơn Trà", "Vinfast Limo Green 2025", 6, 6, "Available", "Automatic", null, null, 723000m, (short)2025, 1, 42734, "Silver" },
                    { 99, 9, 48, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 818000m, 20000m, 2000000m, "Excellent Mazda 3 Deluxe 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90098", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mazda 3 Deluxe 2022", 6, 4, "Available", "Automatic", null, null, 102250m, (short)2022, 3, 43017, "Gray" },
                    { 100, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 523000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90099", "Phường Bình Thuận, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2025, 3, 43300, "Red" },
                    { 101, 13, 67, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1104000m, 20000m, 3000000m, "Excellent Mitsubishi Outlander Premium 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 88000m, "43A-90100", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Mitsubishi Outlander Premium 2020", 6, 7, "Available", "Automatic", null, null, 752000m, (short)2020, 4, 43583, "Blue" },
                    { 102, 6, 29, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 918000m, 20000m, 2500000m, "Excellent Hyundai Venue 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90101", "Phường Hòa Khê, Quận Thanh Khê", "Hyundai Venue 2025", 6, 5, "Available", "Automatic", null, null, 551000m, (short)2025, 1, 43866, "White" },
                    { 103, 9, 54, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1148000m, 20000m, 3000000m, "Excellent Mazda Cx5 Premium 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 92000m, "43A-90102", "Phường Nam Dương, Quận Hải Châu", "Mazda Cx5 Premium 2019", 6, 5, "Available", "Automatic", null, null, 143500m, (short)2019, 2, 44149, "Black" },
                    { 104, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 545000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 44000m, "43A-90103", "Phường Hòa Cường Bắc, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 358000m, (short)2025, 3, 44432, "Silver" },
                    { 105, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 890000m, 20000m, 2000000m, "Excellent Toyota Veloz Cross 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 71000m, "43A-90104", "Phường An Khê, Quận Thanh Khê", "Toyota Veloz Cross 2022", 6, 7, "Available", "Automatic", null, null, 606000m, (short)2022, 4, 44715, "Gray" },
                    { 106, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 529000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90105", "Phường Hòa An, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 348000m, (short)2025, 3, 44998, "Red" },
                    { 107, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 526000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 42000m, "43A-90106", "Phường Hòa Thuận Tây, Quận Hải Châu", "Mitsubishi Attrage 2023", 6, 5, "Available", "Manual", null, null, 65750m, (short)2023, 1, 45281, "Blue" },
                    { 108, 3, 7, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1027000m, 20000m, 2500000m, "Excellent Ford Ranger Xls 4X4 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 82000m, "43A-90107", "Phường Hòa An, Quận Cẩm Lệ", "Ford Ranger Xls 4X4 2025", 6, 5, "Available", "Automatic", null, null, 688000m, (short)2025, 1, 45564, "White" },
                    { 109, 17, 77, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 531000m, 20000m, 1500000m, "Excellent Suzuki Ciaz 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 42000m, "43A-90108", "Phường Hòa Cường Bắc, Quận Hải Châu", "Suzuki Ciaz 2017", 6, 5, "Available", "Automatic", null, null, 379000m, (short)2017, 1, 45847, "Black" },
                    { 110, 8, 38, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1372000m, 20000m, 3500000m, "Excellent Kia Sedona Premium 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 110000m, "43A-90109", "Phường Hòa Cường Nam, Quận Hải Châu", "Kia Sedona Premium 2019", 6, 7, "Available", "Automatic", null, null, 171500m, (short)2019, 4, 46130, "Silver" },
                    { 111, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90110", "Phường Hòa Cường Bắc, Quận Hải Châu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2025, 3, 46413, "Gray" },
                    { 112, 5, 11, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 646000m, 20000m, 1500000m, "Excellent Honda City 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 52000m, "43A-90111", "Phường Hòa Khê, Quận Thanh Khê", "Honda City 2018", 6, 5, "Available", "Automatic", null, null, 448000m, (short)2018, 1, 46696, "Red" },
                    { 113, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1183000m, 20000m, 3000000m, "Excellent Vinfast Vf6 Plus 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 95000m, "43A-90112", "Phường Khuê Trung, Quận Cẩm Lệ", "Vinfast Vf6 Plus 2026", 6, 4, "Available", "Automatic", null, null, 758000m, (short)2026, 3, 46979, "Blue" },
                    { 114, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 531000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 42000m, "43A-90113", "Phường Hòa Cường Bắc, Quận Hải Châu", "Mitsubishi Attrage 2022", 6, 5, "Available", "Manual", null, null, 66375m, (short)2022, 1, 47262, "White" },
                    { 115, 12, 61, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 704000m, 20000m, 2000000m, "Excellent Mg5 Luxury 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 56000m, "43A-90114", "Phường Khuê Trung, Quận Cẩm Lệ", "Mg5 Luxury 2024", 6, 5, "Available", "Automatic", null, null, 482000m, (short)2024, 1, 47545, "Black" },
                    { 116, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 582000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 47000m, "43A-90115", "Phường Thanh Khê Đông, Quận Thanh Khê", "Vinfast Vf3 2026", 6, 4, "Available", "Automatic", null, null, 382000m, (short)2026, 3, 47828, "Silver" },
                    { 117, 9, 47, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 677000m, 20000m, 1500000m, "Excellent Mazda 2 Luxury 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 54000m, "43A-90116", "Phường Hòa Cường Bắc, Quận Hải Châu", "Mazda 2 Luxury 2022", 6, 4, "Available", "Automatic", null, null, 84625m, (short)2022, 3, 48111, "Gray" },
                    { 118, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1154000m, 20000m, 3000000m, "Excellent Vinfast Limo Green 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 92000m, "43A-90117", "Phường Khuê Trung, Quận Cẩm Lệ", "Vinfast Limo Green 2026", 6, 7, "Available", "Automatic", null, null, 740000m, (short)2026, 4, 48394, "Red" },
                    { 119, 9, 46, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 818000m, 20000m, 2000000m, "Excellent Mazda 2 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90118", "Phường Hòa Cường Nam, Quận Hải Châu", "Mazda 2 2025", 6, 4, "Available", "Automatic", null, null, 551000m, (short)2025, 3, 48677, "Blue" },
                    { 120, 6, 25, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 883000m, 20000m, 2000000m, "Excellent Hyundai Stargazer 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 71000m, "43A-90119", "Phường Khuê Trung, Quận Cẩm Lệ", "Hyundai Stargazer 2025", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2025, 4, 48960, "White" },
                    { 121, 8, 45, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1611000m, 20000m, 4000000m, "Excellent Kia Sportage Signature 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 129000m, "43A-90120", "Phường An Khê, Quận Thanh Khê", "Kia Sportage Signature 2025", 6, 5, "Available", "Automatic", null, null, 1026000m, (short)2025, 1, 49243, "Black" },
                    { 122, 8, 35, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 755000m, 20000m, 2000000m, "Excellent Kia Cerato 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 60000m, "43A-90121", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Kia Cerato 2020", 6, 5, "Available", "Automatic", null, null, 94375m, (short)2020, 1, 49526, "Silver" },
                    { 123, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90122", "Phường Thọ Quang, Quận Sơn Trà", "Mitsubishi Xpander 2020", 6, 7, "Available", "Automatic", null, null, 114125m, (short)2020, 4, 49809, "Gray" },
                    { 124, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 878000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90123", "Phường Hòa An, Quận Cẩm Lệ", "Mitsubishi Xpander 2022", 6, 7, "Available", "Automatic", null, null, 599000m, (short)2022, 4, 50092, "Red" },
                    { 125, 8, 39, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 941000m, 20000m, 2500000m, "Excellent Kia Seltos Luxury 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 75000m, "43A-90124", "Phường Hòa Cường Nam, Quận Hải Châu", "Kia Seltos Luxury 2021", 6, 5, "Available", "Automatic", null, null, 117625m, (short)2021, 1, 50375, "Blue" },
                    { 126, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 798000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90125", "Phường An Hải Bắc, Quận Sơn Trà", "Mitsubishi Xpander 2023", 6, 7, "Available", "Manual", null, null, 551000m, (short)2023, 4, 50658, "White" },
                    { 127, 6, 26, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 911000m, 20000m, 2500000m, "Excellent Hyundai Stargazer Premium 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90126", "Phường Hòa Cường Nam, Quận Hải Châu", "Hyundai Stargazer Premium 2024", 6, 7, "Available", "Automatic", null, null, 619000m, (short)2024, 4, 50941, "Black" },
                    { 128, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 797000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90127", "Phường Hòa An, Quận Cẩm Lệ", "Mitsubishi Xpander 2023", 6, 7, "Available", "Automatic", null, null, 99625m, (short)2023, 4, 51224, "Silver" },
                    { 129, 7, 30, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 897000m, 20000m, 2000000m, "Excellent Isuzu Dmax 4X2 2014 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 72000m, "43A-90128", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Isuzu Dmax 4X2 2014", 6, 5, "Available", "Automatic", null, null, 538000m, (short)2014, 1, 51507, "Gray" },
                    { 130, 6, 29, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1033000m, 20000m, 2500000m, "Excellent Hyundai Venue 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 83000m, "43A-90129", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Hyundai Venue 2024", 6, 5, "Available", "Automatic", null, null, 620000m, (short)2024, 1, 51790, "Red" },
                    { 131, 11, 60, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 762000m, 20000m, 2000000m, "Excellent Mg Zs Standard 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 61000m, "43A-90130", "Phường An Khê, Quận Thanh Khê", "Mg Zs Standard 2025", 6, 5, "Available", "Automatic", null, null, 457000m, (short)2025, 1, 52073, "Blue" },
                    { 132, 11, 59, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 918000m, 20000m, 2500000m, "Excellent Mg Zs Luxury 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90131", "Phường Hòa Cường Nam, Quận Hải Châu", "Mg Zs Luxury 2022", 6, 4, "Available", "Automatic", null, null, 551000m, (short)2022, 3, 52356, "White" },
                    { 133, 18, 88, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1159000m, 20000m, 3000000m, "Excellent Vinfast Lux A 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 93000m, "43A-90132", "Phường Hòa An, Quận Cẩm Lệ", "Vinfast Lux A 2022", 6, 5, "Available", "Automatic", null, null, 144875m, (short)2022, 1, 52639, "Black" },
                    { 134, 5, 11, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 658000m, 20000m, 1500000m, "Excellent Honda City 2015 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 53000m, "43A-90133", "Phường Hòa Minh, Quận Liên Chiểu", "Honda City 2015", 6, 4, "Available", "Automatic", null, null, 455000m, (short)2015, 3, 52922, "Silver" },
                    { 135, 8, 31, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1097000m, 20000m, 2500000m, "Excellent Kia Carens 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 88000m, "43A-90134", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Kia Carens 2024", 6, 7, "Available", "Automatic", null, null, 137125m, (short)2024, 4, 53205, "Gray" },
                    { 136, 14, 71, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 646000m, 20000m, 1500000m, "Excellent Nissan Almera El 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 52000m, "43A-90135", "Phường Khuê Trung, Quận Cẩm Lệ", "Nissan Almera El 2022", 6, 5, "Available", "Automatic", null, null, 448000m, (short)2022, 1, 53488, "Red" },
                    { 137, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1132000m, 20000m, 3000000m, "Excellent Vinfast Vf6 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 91000m, "43A-90136", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf6 Plus 2025", 6, 5, "Available", "Automatic", null, null, 727000m, (short)2025, 1, 53771, "Blue" },
                    { 138, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 545000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 44000m, "43A-90137", "Phường Mỹ An, Quận Ngũ Hành Sơn", "Vinfast Vf3 2026", 6, 4, "Available", "Automatic", null, null, 358000m, (short)2026, 3, 54054, "White" },
                    { 139, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 561000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 45000m, "43A-90138", "Phường Thọ Quang, Quận Sơn Trà", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 369000m, (short)2024, 3, 54337, "Black" },
                    { 140, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 738000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 59000m, "43A-90139", "Phường Hòa An, Quận Cẩm Lệ", "Vinfast Vf5 2025", 6, 4, "Available", "Automatic", null, null, 92250m, (short)2025, 3, 54620, "Silver" },
                    { 141, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90140", "Phường Thanh Khê Đông, Quận Thanh Khê", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 379000m, (short)2025, 3, 54903, "Gray" },
                    { 142, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1055000m, 20000m, 2500000m, "Excellent Vinfast Vf6 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 84000m, "43A-90141", "Phường Hòa Cường Bắc, Quận Hải Châu", "Vinfast Vf6 Plus 2025", 6, 4, "Available", "Automatic", null, null, 681000m, (short)2025, 3, 55186, "Red" },
                    { 143, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Toyota Veloz Cross 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90142", "Phường Hòa An, Quận Cẩm Lệ", "Toyota Veloz Cross 2024", 6, 7, "Available", "Automatic", null, null, 114125m, (short)2024, 4, 55469, "Blue" },
                    { 144, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1039000m, 20000m, 2500000m, "Excellent Toyota Veloz Cross 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 83000m, "43A-90143", "Phường Hòa An, Quận Cẩm Lệ", "Toyota Veloz Cross 2025", 6, 7, "Available", "Automatic", null, null, 129875m, (short)2025, 4, 55752, "White" },
                    { 145, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 856000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 68000m, "43A-90144", "Phường Hòa An, Quận Cẩm Lệ", "Mitsubishi Xpander 2023", 6, 7, "Available", "Automatic", null, null, 107000m, (short)2023, 4, 56035, "Black" },
                    { 146, 18, 95, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1246000m, 20000m, 3000000m, "Excellent Vinfast Vf7 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 100000m, "43A-90145", "Phường Thanh Khê Đông, Quận Thanh Khê", "Vinfast Vf7 2025", 6, 4, "Available", "Automatic", null, null, 796000m, (short)2025, 3, 56318, "Silver" },
                    { 147, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 936000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 75000m, "43A-90146", "Phường Hòa An, Quận Cẩm Lệ", "Mitsubishi Xpander 2024", 6, 7, "Available", "Automatic", null, null, 634000m, (short)2024, 4, 56601, "Gray" },
                    { 148, 6, 22, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 795000m, 20000m, 2000000m, "Excellent Hyundai Elantra 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90147", "Phường Hòa Cường Nam, Quận Hải Châu", "Hyundai Elantra 2021", 6, 4, "Available", "Manual", null, null, 537000m, (short)2021, 3, 56884, "Red" },
                    { 149, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 933000m, 20000m, 2500000m, "Excellent Toyota Vios 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 75000m, "43A-90148", "Phường Khuê Trung, Quận Cẩm Lệ", "Toyota Vios 2024", 6, 5, "Available", "Manual", null, null, 116625m, (short)2024, 1, 57167, "Blue" },
                    { 150, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 948000m, 20000m, 2500000m, "Excellent Toyota Vios 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 76000m, "43A-90149", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Toyota Vios 2025", 6, 5, "Available", "Automatic", null, null, 629000m, (short)2025, 1, 57450, "White" },
                    { 151, 8, 34, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1926000m, 20000m, 5000000m, "Excellent Kia Carnival Premium 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 154000m, "43A-90150", "Phường Thanh Khê Tây, Quận Thanh Khê", "Kia Carnival Premium 2022", 6, 7, "Available", "Automatic", null, null, 240750m, (short)2022, 4, 57733, "Black" },
                    { 152, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90151", "Phường Hòa Phát, Quận Cẩm Lệ", "Vinfast Vf3 2026", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2026, 3, 58016, "Silver" },
                    { 153, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 569000m, 20000m, 1500000m, "Excellent Toyota Vios 2010 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 46000m, "43A-90152", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Toyota Vios 2010", 6, 5, "Available", "Manual", null, null, 402000m, (short)2010, 1, 58299, "Gray" },
                    { 154, 5, 17, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 620000m, 20000m, 1500000m, "Excellent Honda Jazz 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 50000m, "43A-90153", "Phường Khuê Trung, Quận Cẩm Lệ", "Honda Jazz 2018", 6, 4, "Available", "Automatic", null, null, 77500m, (short)2018, 3, 58582, "Red" },
                    { 155, 8, 40, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1085000m, 20000m, 2500000m, "Excellent Kia Seltos Premium 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 87000m, "43A-90154", "Phường Hòa Xuân, Quận Cẩm Lệ", "Kia Seltos Premium 2024", 6, 5, "Available", "Automatic", null, null, 723000m, (short)2024, 1, 58865, "Blue" },
                    { 156, 5, 15, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1246000m, 20000m, 3000000m, "Excellent Honda Crv L Awd 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 100000m, "43A-90155", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Honda Crv L Awd 2018", 6, 7, "Available", "Automatic", null, null, 820000m, (short)2018, 4, 59148, "White" },
                    { 157, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 501000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 40000m, "43A-90156", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 329000m, (short)2025, 3, 59431, "Black" },
                    { 158, 6, 22, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 688000m, 20000m, 1500000m, "Excellent Hyundai Elantra 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 55000m, "43A-90157", "Phường Hòa Xuân, Quận Cẩm Lệ", "Hyundai Elantra 2016", 6, 5, "Available", "Automatic", null, null, 473000m, (short)2016, 1, 59714, "Silver" },
                    { 159, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90158", "Phường Hòa Minh, Quận Liên Chiểu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 379000m, (short)2025, 3, 59997, "Gray" },
                    { 160, 1, 82, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1143000m, 20000m, 3000000m, "Excellent Toyota Fortuner 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 91000m, "43A-90159", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Toyota Fortuner 2019", 6, 8, "Available", "Automatic", null, null, 142875m, (short)2019, 4, 15280, "Red" },
                    { 161, 14, 72, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 646000m, 20000m, 1500000m, "Excellent Nissan Almera Vl 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 52000m, "43A-90160", "Phường Hòa An, Quận Cẩm Lệ", "Nissan Almera Vl 2021", 6, 4, "Available", "Automatic", null, null, 448000m, (short)2021, 3, 15563, "Blue" },
                    { 162, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1284000m, 20000m, 3000000m, "Excellent Vinfast Limo Green 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 103000m, "43A-90161", "Phường Hòa Minh, Quận Liên Chiểu", "Vinfast Limo Green 2025", 6, 7, "Available", "Automatic", null, null, 818000m, (short)2025, 4, 15846, "White" },
                    { 163, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 897000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90162", "Phường Hòa Minh, Quận Liên Chiểu", "Mitsubishi Xpander 2026", 6, 7, "Available", "Automatic", null, null, 610000m, (short)2026, 4, 16129, "Black" },
                    { 164, 5, 13, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Honda Crv G 2015 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90163", "Phường Hòa Khánh Bắc, Quận Liên Chiểu", "Honda Crv G 2015", 6, 5, "Available", "Automatic", null, null, 114125m, (short)2015, 1, 16412, "Silver" },
                    { 165, 3, 6, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 624000m, 20000m, 1500000m, "Excellent Ford Focus 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 50000m, "43A-90164", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Ford Focus 2017", 6, 5, "Available", "Automatic", null, null, 78000m, (short)2017, 1, 16695, "Gray" },
                    { 166, 8, 31, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 624000m, 20000m, 1500000m, "Excellent Kia Carens 2011 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 50000m, "43A-90165", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Kia Carens 2011", 6, 7, "Available", "Manual", null, null, 78000m, (short)2011, 4, 16978, "Red" },
                    { 167, 8, 37, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 322000m, 20000m, 1500000m, "Excellent Kia Morning 2015 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 26000m, "43A-90166", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Kia Morning 2015", 6, 4, "Available", "Manual", null, null, 241000m, (short)2015, 3, 17261, "Blue" },
                    { 168, 13, 64, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Mitsubishi Attrage 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 46000m, "43A-90167", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Mitsubishi Attrage 2020", 6, 5, "Available", "Automatic", null, null, 72000m, (short)2020, 1, 17544, "White" },
                    { 169, 6, 18, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 853000m, 20000m, 2000000m, "Excellent Hyundai Accent 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 68000m, "43A-90168", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Hyundai Accent 2024", 6, 5, "Available", "Automatic", null, null, 572000m, (short)2024, 1, 17827, "Black" },
                    { 170, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1132000m, 20000m, 3000000m, "Excellent Vinfast Vf6 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 91000m, "43A-90169", "Phường Khuê Trung, Quận Cẩm Lệ", "Vinfast Vf6 Plus 2025", 6, 5, "Available", "Automatic", null, null, 727000m, (short)2025, 1, 18110, "Silver" },
                    { 171, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 493000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 39000m, "43A-90170", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 324000m, (short)2025, 3, 18393, "Gray" },
                    { 172, 1, 1, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 566000m, 20000m, 1500000m, "Excellent Toyota Vios 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 45000m, "43A-90171", "Phường Hòa Minh, Quận Liên Chiểu", "Toyota Vios 2020", 6, 5, "Available", "Manual", null, null, 400000m, (short)2020, 1, 18676, "Red" },
                    { 173, 9, 48, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 797000m, 20000m, 2000000m, "Excellent Mazda 3 Deluxe 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 64000m, "43A-90172", "Phường Hòa Minh, Quận Liên Chiểu", "Mazda 3 Deluxe 2019", 6, 5, "Available", "Automatic", null, null, 99625m, (short)2019, 1, 18959, "Blue" },
                    { 174, 1, 86, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 998000m, 20000m, 2500000m, "Excellent Toyota Yaris Cross 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 80000m, "43A-90173", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Toyota Yaris Cross 2024", 6, 5, "Available", "Automatic", null, null, 689000m, (short)2024, 1, 19242, "White" },
                    { 175, 8, 32, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1365000m, 20000m, 3500000m, "Excellent Kia Carens Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 109000m, "43A-90174", "Phường Hòa Khánh Bắc, Quận Liên Chiểu", "Kia Carens Luxury 2025", 6, 7, "Available", "Automatic", null, null, 879000m, (short)2025, 4, 19525, "Black" },
                    { 176, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 783000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 63000m, "43A-90175", "Phường Khuê Trung, Quận Cẩm Lệ", "Vinfast Vf5 2026", 6, 5, "Available", "Automatic", null, null, 515000m, (short)2026, 1, 19808, "Silver" },
                    { 177, 9, 49, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 876000m, 20000m, 2000000m, "Excellent Mazda 3 Luxury 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90176", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Mazda 3 Luxury 2020", 6, 4, "Available", "Automatic", null, null, 109500m, (short)2020, 3, 20091, "Gray" },
                    { 178, 6, 21, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 969000m, 20000m, 2500000m, "Excellent Hyundai Creta Luxury 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 78000m, "43A-90177", "Phường Hòa Minh, Quận Liên Chiểu", "Hyundai Creta Luxury 2023", 6, 5, "Available", "Automatic", null, null, 121125m, (short)2023, 1, 20374, "Red" },
                    { 179, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 814000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90178", "Phường Hòa An, Quận Cẩm Lệ", "Mitsubishi Xpander 2020", 6, 7, "Available", "Automatic", null, null, 101750m, (short)2020, 4, 20657, "Blue" },
                    { 180, 14, 74, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 827000m, 20000m, 2000000m, "Excellent Nissan Navara 4X2 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 66000m, "43A-90179", "Phường Hòa Minh, Quận Liên Chiểu", "Nissan Navara 4X2 2016", 6, 4, "Available", "Automatic", null, null, 568000m, (short)2016, 3, 20940, "White" },
                    { 181, 1, 82, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1522000m, 20000m, 4000000m, "Excellent Toyota Fortuner 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 122000m, "43A-90180", "Phường Hòa Xuân, Quận Cẩm Lệ", "Toyota Fortuner 2023", 6, 7, "Available", "Automatic", null, null, 985000m, (short)2023, 4, 21223, "Black" },
                    { 182, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Toyota Veloz Cross 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90181", "Phường Hòa Thọ Tây, Quận Cẩm Lệ", "Toyota Veloz Cross 2024", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2024, 4, 21506, "Silver" },
                    { 183, 8, 32, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 971000m, 20000m, 2500000m, "Excellent Kia Carens Luxury 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 78000m, "43A-90182", "Phường Hòa Minh, Quận Liên Chiểu", "Kia Carens Luxury 2024", 6, 7, "Available", "Automatic", null, null, 654000m, (short)2024, 4, 21789, "Gray" },
                    { 184, 9, 51, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1037000m, 20000m, 2500000m, "Excellent Mazda 6 Premium 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 83000m, "43A-90183", "Phường Hòa Xuân, Quận Cẩm Lệ", "Mazda 6 Premium 2021", 6, 4, "Available", "Automatic", null, null, 682000m, (short)2021, 3, 22072, "Red" },
                    { 185, 13, 69, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1091000m, 20000m, 2500000m, "Excellent Mitsubishi Xforce Ultimate 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 87000m, "43A-90184", "Phường Hòa Xuân, Quận Cẩm Lệ", "Mitsubishi Xforce Ultimate 2025", 6, 5, "Available", "Automatic", null, null, 654000m, (short)2025, 1, 22355, "Blue" },
                    { 186, 1, 81, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1275000m, 20000m, 3000000m, "Excellent Toyota Corolla Cross V 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 102000m, "43A-90185", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Toyota Corolla Cross V 2025", 6, 5, "Available", "Automatic", null, null, 765000m, (short)2025, 1, 22638, "White" },
                    { 187, 14, 73, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 860000m, 20000m, 2000000m, "Excellent Nissan Navara 2017 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 69000m, "43A-90186", "Phường Khuê Trung, Quận Cẩm Lệ", "Nissan Navara 2017", 6, 5, "Available", "Automatic", null, null, 516000m, (short)2017, 1, 22921, "Black" },
                    { 188, 4, 10, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 917000m, 20000m, 2500000m, "Excellent Geely Coolray Standard 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90187", "Phường Hòa Minh, Quận Liên Chiểu", "Geely Coolray Standard 2025", 6, 5, "Available", "Automatic", null, null, 114625m, (short)2025, 1, 23204, "Silver" },
                    { 189, 6, 20, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 922000m, 20000m, 2500000m, "Excellent Hyundai Creta 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 74000m, "43A-90188", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Hyundai Creta 2023", 6, 5, "Available", "Automatic", null, null, 553000m, (short)2023, 1, 23487, "Gray" },
                    { 190, 13, 68, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1147000m, 20000m, 3000000m, "Excellent Mitsubishi Pajero 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 92000m, "43A-90189", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Mitsubishi Pajero 2018", 6, 7, "Available", "Manual", null, null, 143375m, (short)2018, 4, 23770, "Red" },
                    { 191, 5, 16, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1263000m, 20000m, 3000000m, "Excellent Honda Hrv G 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 101000m, "43A-90190", "Phường Hòa An, Quận Cẩm Lệ", "Honda Hrv G 2023", 6, 5, "Available", "Automatic", null, null, 758000m, (short)2023, 1, 24053, "Blue" },
                    { 192, 5, 14, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1458000m, 20000m, 3500000m, "Excellent Honda Crv L 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 117000m, "43A-90191", "Phường Hòa Minh, Quận Liên Chiểu", "Honda Crv L 2022", 6, 7, "Available", "Automatic", null, null, 875000m, (short)2022, 4, 24336, "White" },
                    { 193, 8, 41, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 589000m, 20000m, 1500000m, "Excellent Kia Soluto 2019 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 47000m, "43A-90192", "Phường Hòa Minh, Quận Liên Chiểu", "Kia Soluto 2019", 6, 5, "Available", "Automatic", null, null, 413000m, (short)2019, 1, 24619, "Black" },
                    { 194, 6, 18, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 715000m, 20000m, 2000000m, "Excellent Hyundai Accent 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 57000m, "43A-90193", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Hyundai Accent 2023", 6, 5, "Available", "Automatic", null, null, 89375m, (short)2023, 1, 24902, "Silver" },
                    { 195, 6, 22, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 818000m, 20000m, 2000000m, "Excellent Hyundai Elantra 2018 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 65000m, "43A-90194", "Phường Hòa Xuân, Quận Cẩm Lệ", "Hyundai Elantra 2018", 6, 4, "Available", "Automatic", null, null, 551000m, (short)2018, 3, 25185, "Gray" },
                    { 196, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90195", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2025, 3, 25468, "Red" },
                    { 197, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 465000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 37000m, "43A-90196", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 306000m, (short)2024, 3, 25751, "Blue" },
                    { 198, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 468000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 37000m, "43A-90197", "Phường Khuê Mỹ, Quận Ngũ Hành Sơn", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 308000m, (short)2024, 3, 26034, "White" },
                    { 199, 1, 85, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Toyota Veloz Cross 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90198", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Toyota Veloz Cross 2022", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2022, 4, 26317, "Black" },
                    { 200, 6, 24, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1338000m, 20000m, 3500000m, "Excellent Hyundai Santafe 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 107000m, "43A-90199", "Phường Hòa Xuân, Quận Cẩm Lệ", "Hyundai Santafe 2020", 6, 7, "Available", "Automatic", null, null, 875000m, (short)2020, 4, 26600, "Silver" },
                    { 201, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 734000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 59000m, "43A-90200", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf5 2025", 6, 5, "Available", "Automatic", null, null, 482000m, (short)2025, 1, 26883, "Gray" },
                    { 202, 1, 82, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1113000m, 20000m, 3000000m, "Excellent Toyota Fortuner 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 89000m, "43A-90201", "Phường Hòa Xuân, Quận Cẩm Lệ", "Toyota Fortuner 2021", 6, 7, "Available", "Automatic", null, null, 139125m, (short)2021, 4, 27166, "Red" },
                    { 203, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1068000m, 20000m, 2500000m, "Excellent Vinfast Vf6 Plus 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 85000m, "43A-90202", "Phường Hòa Minh, Quận Liên Chiểu", "Vinfast Vf6 Plus 2024", 6, 4, "Available", "Automatic", null, null, 133500m, (short)2024, 3, 27449, "Blue" },
                    { 204, 8, 35, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 899000m, 20000m, 2000000m, "Excellent Kia Cerato 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90203", "Phường Hòa Minh, Quận Liên Chiểu", "Kia Cerato 2020", 6, 5, "Available", "Automatic", null, null, 112375m, (short)2020, 1, 27732, "White" },
                    { 205, 18, 87, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1195000m, 20000m, 3000000m, "Excellent Vinfast Limo Green 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 96000m, "43A-90204", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Vinfast Limo Green 2026", 6, 7, "Available", "Automatic", null, null, 149375m, (short)2026, 4, 28015, "Black" },
                    { 206, 8, 32, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1200000m, 20000m, 3000000m, "Excellent Kia Carens Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 96000m, "43A-90205", "Phường Khuê Trung, Quận Cẩm Lệ", "Kia Carens Luxury 2025", 6, 7, "Available", "Automatic", null, null, 792000m, (short)2025, 4, 28298, "Silver" },
                    { 207, 17, 78, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 775000m, 20000m, 2000000m, "Excellent Suzuki Ertiga 2020 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 62000m, "43A-90206", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Suzuki Ertiga 2020", 6, 7, "Available", "Automatic", null, null, 96875m, (short)2020, 4, 28581, "Gray" },
                    { 208, 4, 9, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 896000m, 20000m, 2000000m, "Excellent Geely Coolray Flagship 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90207", "Xã Hòa Phước, Huyện Hòa Vang", "Geely Coolray Flagship 2025", 6, 5, "Available", "Automatic", null, null, 112000m, (short)2025, 1, 28864, "Red" },
                    { 209, 4, 9, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 896000m, 20000m, 2000000m, "Excellent Geely Coolray Flagship 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90208", "Xã Hòa Châu, Huyện Hòa Vang", "Geely Coolray Flagship 2025", 6, 5, "Available", "Automatic", null, null, 112000m, (short)2025, 1, 29147, "Blue" },
                    { 210, 4, 9, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 896000m, 20000m, 2000000m, "Excellent Geely Coolray Flagship 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 72000m, "43A-90209", "Xã Hòa Phước, Huyện Hòa Vang", "Geely Coolray Flagship 2025", 6, 5, "Available", "Automatic", null, null, 112000m, (short)2025, 1, 29430, "White" },
                    { 211, 6, 19, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 497000m, 20000m, 1500000m, "Excellent Hyundai Avante 2013 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 40000m, "43A-90210", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Hyundai Avante 2013", 6, 5, "Available", "Automatic", null, null, 358000m, (short)2013, 1, 29713, "Black" },
                    { 212, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90211", "Xã Hòa Phước, Huyện Hòa Vang", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 72000m, (short)2025, 3, 29996, "Silver" },
                    { 213, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 524000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 42000m, "43A-90212", "Phường Hòa Khánh Bắc, Quận Liên Chiểu", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 344000m, (short)2025, 3, 30279, "Gray" },
                    { 214, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1068000m, 20000m, 2500000m, "Excellent Vinfast Vf6 Plus 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 85000m, "43A-90213", "Phường Hòa Thọ Đông, Quận Cẩm Lệ", "Vinfast Vf6 Plus 2026", 6, 5, "Available", "Automatic", null, null, 689000m, (short)2026, 1, 30562, "Red" },
                    { 215, 8, 34, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 2084000m, 20000m, 5000000m, "Excellent Kia Carnival Premium 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Diesel", 167000m, "43A-90214", "Phường Hòa Xuân, Quận Cẩm Lệ", "Kia Carnival Premium 2024", 6, 8, "Available", "Automatic", null, null, 1322000m, (short)2024, 4, 30845, "Blue" },
                    { 216, 18, 94, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1132000m, 20000m, 3000000m, "Excellent Vinfast Vf6 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 91000m, "43A-90215", "Phường Hòa Khánh Bắc, Quận Liên Chiểu", "Vinfast Vf6 Plus 2025", 6, 5, "Available", "Automatic", null, null, 727000m, (short)2025, 1, 31128, "White" },
                    { 217, 5, 12, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 822000m, 20000m, 2000000m, "Excellent Honda City Rs 2021 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 66000m, "43A-90216", "Phường Hòa Xuân, Quận Cẩm Lệ", "Honda City Rs 2021", 6, 5, "Available", "Automatic", null, null, 553000m, (short)2021, 1, 31411, "Black" },
                    { 218, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90217", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 379000m, (short)2025, 3, 31694, "Silver" },
                    { 219, 18, 93, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1073000m, 20000m, 2500000m, "Excellent Vinfast Vf6 Eco 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 86000m, "43A-90218", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Vinfast Vf6 Eco 2024", 6, 4, "Available", "Automatic", null, null, 644000m, (short)2024, 3, 31977, "Gray" },
                    { 220, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90219", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 379000m, (short)2025, 3, 32260, "Red" },
                    { 221, 6, 27, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 710000m, 20000m, 2000000m, "Excellent Hyundai Tucson Premium 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 57000m, "43A-90220", "Phường Hòa Xuân, Quận Cẩm Lệ", "Hyundai Tucson Premium 2016", 6, 5, "Available", "Automatic", null, null, 516000m, (short)2016, 2, 32543, "Blue" },
                    { 222, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 576000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 46000m, "43A-90221", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf3 2025", 6, 4, "Available", "Automatic", null, null, 72000m, (short)2025, 3, 32826, "White" },
                    { 223, 18, 92, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 844000m, 20000m, 2000000m, "Excellent Vinfast Vf5 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 68000m, "43A-90222", "Phường Hòa Xuân, Quận Cẩm Lệ", "Vinfast Vf5 2024", 6, 5, "Available", "Automatic", null, null, 105500m, (short)2024, 1, 33109, "Black" },
                    { 224, 18, 97, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1326000m, 20000m, 3500000m, "Excellent Vinfast Vf7 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 106000m, "43A-90223", "Xã Hòa Châu, Huyện Hòa Vang", "Vinfast Vf7 Plus 2025", 6, 5, "Available", "Automatic", null, null, 165750m, (short)2025, 1, 33392, "Silver" },
                    { 225, 8, 32, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1143000m, 20000m, 3000000m, "Excellent Kia Carens Luxury 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 91000m, "43A-90224", "Phường Hòa Thọ Tây, Quận Cẩm Lệ", "Kia Carens Luxury 2025", 6, 7, "Available", "Automatic", null, null, 758000m, (short)2025, 4, 33675, "Gray" },
                    { 226, 18, 90, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 472000m, 20000m, 1500000m, "Excellent Vinfast Minio Green 2026 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 38000m, "43A-90225", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Vinfast Minio Green 2026", 6, 4, "Available", "Automatic", null, null, 310000m, (short)2026, 3, 33958, "Red" },
                    { 227, 18, 91, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 472000m, 20000m, 1500000m, "Excellent Vinfast Vf3 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 38000m, "43A-90226", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Vinfast Vf3 2024", 6, 4, "Available", "Automatic", null, null, 310000m, (short)2024, 3, 34241, "Blue" },
                    { 228, 8, 35, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 497000m, 20000m, 1500000m, "Excellent Kia Cerato 2010 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 40000m, "43A-90227", "Phường Hòa Xuân, Quận Cẩm Lệ", "Kia Cerato 2010", 6, 4, "Available", "Automatic", null, null, 358000m, (short)2010, 3, 34524, "White" },
                    { 229, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 913000m, 20000m, 2500000m, "Excellent Mitsubishi Xpander 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90228", "Phường Hoà Quý, Quận Ngũ Hành Sơn", "Mitsubishi Xpander 2023", 6, 7, "Available", "Automatic", null, null, 620000m, (short)2023, 4, 34807, "Black" },
                    { 230, 18, 97, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1298000m, 20000m, 3000000m, "Excellent Vinfast Vf7 Plus 2025 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Electric", 104000m, "43A-90229", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Vinfast Vf7 Plus 2025", 6, 4, "Available", "Automatic", null, null, 827000m, (short)2025, 3, 35090, "Silver" },
                    { 231, 11, 59, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 917000m, 20000m, 2500000m, "Excellent Mg Zs Luxury 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 73000m, "43A-90230", "Phường Hoà Quý, Quận Ngũ Hành Sơn", "Mg Zs Luxury 2022", 6, 5, "Available", "Automatic", null, null, 550000m, (short)2022, 1, 35373, "Gray" },
                    { 232, 6, 18, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 746000m, 20000m, 2000000m, "Excellent Hyundai Accent 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 60000m, "43A-90231", "Phường Hòa Khánh Bắc, Quận Liên Chiểu", "Hyundai Accent 2024", 6, 5, "Available", "Manual", null, null, 93250m, (short)2024, 1, 35656, "Red" },
                    { 233, 1, 82, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 926000m, 20000m, 2500000m, "Excellent Toyota Fortuner 2014 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 74000m, "43A-90232", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Toyota Fortuner 2014", 6, 7, "Available", "Automatic", null, null, 627000m, (short)2014, 4, 35939, "Blue" },
                    { 234, 6, 23, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 430000m, 20000m, 1500000m, "Excellent Hyundai I10 Sedan 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 34000m, "43A-90233", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Hyundai I10 Sedan 2016", 6, 5, "Available", "Manual", null, null, 318000m, (short)2016, 1, 36222, "White" },
                    { 235, 10, 57, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1965000m, 20000m, 5000000m, "Excellent Mercedes C200 Exclusive 2023 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 157000m, "43A-90234", "Phường Hòa Khánh Nam, Quận Liên Chiểu", "Mercedes C200 Exclusive 2023", 6, 4, "Available", "Automatic", null, null, 1239000m, (short)2023, 3, 36505, "Black" },
                    { 236, 10, 58, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1277000m, 20000m, 3000000m, "Excellent Mercedes C250 2016 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 102000m, "43A-90235", "Phường Hoà Hải, Quận Ngũ Hành Sơn", "Mercedes C250 2016", 6, 5, "Available", "Automatic", null, null, 856000m, (short)2016, 1, 36788, "Silver" },
                    { 237, 12, 62, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 876000m, 20000m, 2000000m, "Excellent Mg5 Mg5 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 70000m, "43A-90236", "Xã Hòa Phước, Huyện Hòa Vang", "Mg5 Mg5 2024", 6, 5, "Available", "Automatic", null, null, 109500m, (short)2024, 1, 37071, "Gray" },
                    { 238, 1, 86, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 1074000m, 20000m, 2500000m, "Excellent Toyota Yaris Cross 2024 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 86000m, "43A-90237", "Xã Hòa Phước, Huyện Hòa Vang", "Toyota Yaris Cross 2024", 6, 4, "Available", "Automatic", null, null, 716000m, (short)2024, 3, 37354, "Red" },
                    { 239, 9, 49, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 986000m, 20000m, 2500000m, "Excellent Mazda 3 Luxury 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 79000m, "43A-90238", "Xã Hòa Phong, Huyện Hòa Vang", "Mazda 3 Luxury 2022", 6, 5, "Available", "Automatic", null, null, 652000m, (short)2022, 1, 37637, "Blue" },
                    { 240, 6, 18, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 508000m, 20000m, 1500000m, "Excellent Hyundai Accent 2012 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 41000m, "43A-90239", "Xã Hòa Châu, Huyện Hòa Vang", "Hyundai Accent 2012", 6, 5, "Available", "Manual", null, null, 365000m, (short)2012, 1, 37920, "White" },
                    { 241, 13, 70, new DateTime(2026, 7, 11, 0, 0, 0, 0, DateTimeKind.Utc), 711000m, 20000m, 2000000m, "Excellent Mitsubishi Xpander 2022 in prime condition. Features clean interior, smooth transmission, and outstanding fuel economy. Sanitized after every booking, perfect for exploring Da Nang.", "Gasoline", 57000m, "43A-90240", "Xã Hòa Sơn, Huyện Hòa Vang", "Mitsubishi Xpander 2022", 6, 7, "Available", "Manual", null, null, 88875m, (short)2022, 4, 38203, "Black" }
                });

            // CarImages: 1 placeholder per car, replace with Cloudinary URLs after upload
            migrationBuilder.InsertData(
                table: "CarImages",
                columns: new[] { "Id", "CarId", "DisplayOrder", "IsPrimary", "ImageUrl" },
                values: new object[,]
                {
                    { 1, 2, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2019" },
                    { 2, 3, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=chevrolet-spark-2012" },
                    { 3, 4, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 4, 5, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 5, 6, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg5-luxury-2024" },
                    { 6, 7, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-k3-premium-2022" },
                    { 7, 8, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-innova-cross-2025" },
                    { 8, 9, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2026" },
                    { 9, 10, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2022" },
                    { 10, 11, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=chevrolet-cruze-2015" },
                    { 11, 12, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-lux-sa-2021" },
                    { 12, 13, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf7-eco-2026" },
                    { 13, 14, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2025" },
                    { 14, 15, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 15, 16, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2024" },
                    { 16, 17, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2022" },
                    { 17, 18, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-soluto-2020" },
                    { 18, 19, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2026" },
                    { 19, 20, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-innova-2018" },
                    { 20, 21, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=chevrolet-captiva-2016" },
                    { 21, 22, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-stargazer-2024" },
                    { 22, 23, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-cerato-2019" },
                    { 23, 24, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-luxury-2020" },
                    { 24, 25, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=chevrolet-colorado-4x2-2018" },
                    { 25, 26, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-standard-2024" },
                    { 26, 27, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=omoda-c5-flagship-turbo-2025" },
                    { 27, 28, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx5-luxury-2024" },
                    { 28, 29, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-standard-2021" },
                    { 29, 30, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2020" },
                    { 30, 31, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carnival-premium-2021" },
                    { 31, 32, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-fortuner-2009" },
                    { 32, 33, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2024" },
                    { 33, 34, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2018" },
                    { 34, 35, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-deluxe-2017" },
                    { 35, 36, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 36, 37, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-outlander-2019" },
                    { 37, 38, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg5-standard-2025" },
                    { 38, 39, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-avanza-2023" },
                    { 39, 40, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2018" },
                    { 40, 41, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-2-2024" },
                    { 41, 42, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2019" },
                    { 42, 43, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-veloser-hatchback-2011" },
                    { 43, 44, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=suzuki-swift-hatchback-2016" },
                    { 44, 45, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2024" },
                    { 45, 46, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-rs-2025" },
                    { 46, 47, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-lux-a-2021" },
                    { 47, 48, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2024" },
                    { 48, 49, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 49, 50, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-santafe-2024" },
                    { 50, 51, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-luxury-2024" },
                    { 51, 52, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-sorento-premium-2018" },
                    { 52, 53, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 53, 54, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2026" },
                    { 54, 55, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-2016" },
                    { 55, 56, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2024" },
                    { 56, 57, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-outlander-2018" },
                    { 57, 58, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2025" },
                    { 58, 59, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-accent-2020" },
                    { 59, 60, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-innova-2019" },
                    { 60, 61, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-soluto-2021" },
                    { 61, 62, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-innova-2017" },
                    { 62, 63, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 63, 64, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2020" },
                    { 64, 65, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2019" },
                    { 65, 66, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 66, 67, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 67, 68, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 68, 69, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2022" },
                    { 69, 70, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-eco-2024" },
                    { 70, 71, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2022" },
                    { 71, 72, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mercedes-c200-2008" },
                    { 72, 73, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2022" },
                    { 73, 74, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-seltos-luxury-2021" },
                    { 74, 75, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-mirage-2018" },
                    { 75, 76, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2020" },
                    { 76, 77, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-sonet-luxury-2025" },
                    { 77, 78, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-outlander-2022" },
                    { 78, 79, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2021" },
                    { 79, 80, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2024" },
                    { 80, 81, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-premium-2017" },
                    { 81, 82, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-sorento-luxury-2015" },
                    { 82, 83, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-2025" },
                    { 83, 84, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-2025" },
                    { 84, 85, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=peugeot-3008-2018" },
                    { 85, 86, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-creta-luxury-2025" },
                    { 86, 87, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx5-deluxe-2018" },
                    { 87, 88, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=ford-territory-titanium-2023" },
                    { 88, 89, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx5-deluxe-2024" },
                    { 89, 90, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx5-luxury-2025" },
                    { 90, 91, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-creta-2022" },
                    { 91, 92, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-standard-2024" },
                    { 92, 93, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-premium-2023" },
                    { 93, 94, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-2-2024" },
                    { 94, 95, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-2024" },
                    { 95, 96, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx8-premium-2023" },
                    { 96, 97, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2025" },
                    { 97, 98, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2025" },
                    { 98, 99, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-deluxe-2022" },
                    { 99, 100, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 100, 101, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-outlander-premium-2020" },
                    { 101, 102, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-venue-2025" },
                    { 102, 103, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-cx5-premium-2019" },
                    { 103, 104, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 104, 105, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2022" },
                    { 105, 106, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 106, 107, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2023" },
                    { 107, 108, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=ford-ranger-xls-4x4-2025" },
                    { 108, 109, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=suzuki-ciaz-2017" },
                    { 109, 110, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-sedona-premium-2019" },
                    { 110, 111, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 111, 112, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-2018" },
                    { 112, 113, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2026" },
                    { 113, 114, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2022" },
                    { 114, 115, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg5-luxury-2024" },
                    { 115, 116, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2026" },
                    { 116, 117, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-2-luxury-2022" },
                    { 117, 118, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2026" },
                    { 118, 119, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-2-2025" },
                    { 119, 120, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-stargazer-2025" },
                    { 120, 121, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-sportage-signature-2025" },
                    { 121, 122, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-cerato-2020" },
                    { 122, 123, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2020" },
                    { 123, 124, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2022" },
                    { 124, 125, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-seltos-luxury-2021" },
                    { 125, 126, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 126, 127, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-stargazer-premium-2024" },
                    { 127, 128, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 128, 129, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=isuzu-dmax-4x2-2014" },
                    { 129, 130, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-venue-2024" },
                    { 130, 131, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-standard-2025" },
                    { 131, 132, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-luxury-2022" },
                    { 132, 133, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-lux-a-2022" },
                    { 133, 134, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-2015" },
                    { 134, 135, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-2024" },
                    { 135, 136, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=nissan-almera-el-2022" },
                    { 136, 137, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2025" },
                    { 137, 138, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2026" },
                    { 138, 139, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 139, 140, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2025" },
                    { 140, 141, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 141, 142, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2025" },
                    { 142, 143, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2024" },
                    { 143, 144, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2025" },
                    { 144, 145, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 145, 146, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf7-2025" },
                    { 146, 147, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2024" },
                    { 147, 148, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-elantra-2021" },
                    { 148, 149, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2024" },
                    { 149, 150, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2025" },
                    { 150, 151, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carnival-premium-2022" },
                    { 151, 152, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2026" },
                    { 152, 153, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2010" },
                    { 153, 154, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-jazz-2018" },
                    { 154, 155, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-seltos-premium-2024" },
                    { 155, 156, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-crv-l-awd-2018" },
                    { 156, 157, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 157, 158, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-elantra-2016" },
                    { 158, 159, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 159, 160, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-fortuner-2019" },
                    { 160, 161, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=nissan-almera-vl-2021" },
                    { 161, 162, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2025" },
                    { 162, 163, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2026" },
                    { 163, 164, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-crv-g-2015" },
                    { 164, 165, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=ford-focus-2017" },
                    { 165, 166, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-2011" },
                    { 166, 167, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-morning-2015" },
                    { 167, 168, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-attrage-2020" },
                    { 168, 169, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-accent-2024" },
                    { 169, 170, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2025" },
                    { 170, 171, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 171, 172, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-vios-2020" },
                    { 172, 173, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-deluxe-2019" },
                    { 173, 174, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-yaris-cross-2024" },
                    { 174, 175, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-luxury-2025" },
                    { 175, 176, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2026" },
                    { 176, 177, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-luxury-2020" },
                    { 177, 178, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-creta-luxury-2023" },
                    { 178, 179, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2020" },
                    { 179, 180, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=nissan-navara-4x2-2016" },
                    { 180, 181, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-fortuner-2023" },
                    { 181, 182, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2024" },
                    { 182, 183, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-luxury-2024" },
                    { 183, 184, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-6-premium-2021" },
                    { 184, 185, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xforce-ultimate-2025" },
                    { 185, 186, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-corolla-cross-v-2025" },
                    { 186, 187, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=nissan-navara-2017" },
                    { 187, 188, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=geely-coolray-standard-2025" },
                    { 188, 189, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-creta-2023" },
                    { 189, 190, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-pajero-2018" },
                    { 190, 191, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-hrv-g-2023" },
                    { 191, 192, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-crv-l-2022" },
                    { 192, 193, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-soluto-2019" },
                    { 193, 194, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-accent-2023" },
                    { 194, 195, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-elantra-2018" },
                    { 195, 196, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 196, 197, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 197, 198, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 198, 199, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-veloz-cross-2022" },
                    { 199, 200, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-santafe-2020" },
                    { 200, 201, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2025" },
                    { 201, 202, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-fortuner-2021" },
                    { 202, 203, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2024" },
                    { 203, 204, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-cerato-2020" },
                    { 204, 205, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-limo-green-2026" },
                    { 205, 206, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-luxury-2025" },
                    { 206, 207, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=suzuki-ertiga-2020" },
                    { 207, 208, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=geely-coolray-flagship-2025" },
                    { 208, 209, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=geely-coolray-flagship-2025" },
                    { 209, 210, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=geely-coolray-flagship-2025" },
                    { 210, 211, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-avante-2013" },
                    { 211, 212, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 212, 213, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 213, 214, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2026" },
                    { 214, 215, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carnival-premium-2024" },
                    { 215, 216, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-plus-2025" },
                    { 216, 217, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=honda-city-rs-2021" },
                    { 217, 218, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 218, 219, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf6-eco-2024" },
                    { 219, 220, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 220, 221, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-tucson-premium-2016" },
                    { 221, 222, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2025" },
                    { 222, 223, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf5-2024" },
                    { 223, 224, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf7-plus-2025" },
                    { 224, 225, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-carens-luxury-2025" },
                    { 225, 226, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-minio-green-2026" },
                    { 226, 227, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf3-2024" },
                    { 227, 228, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=kia-cerato-2010" },
                    { 228, 229, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2023" },
                    { 229, 230, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=vinfast-vf7-plus-2025" },
                    { 230, 231, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg-zs-luxury-2022" },
                    { 231, 232, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-accent-2024" },
                    { 232, 233, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-fortuner-2014" },
                    { 233, 234, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-i10-sedan-2016" },
                    { 234, 235, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mercedes-c200-exclusive-2023" },
                    { 235, 236, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mercedes-c250-2016" },
                    { 236, 237, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mg5-mg5-2024" },
                    { 237, 238, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=toyota-yaris-cross-2024" },
                    { 238, 239, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mazda-3-luxury-2022" },
                    { 239, 240, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=hyundai-accent-2012" },
                    { 240, 241, 0, true, "https://placehold.co/800x500/e2e8f0/475569?text=mitsubishi-xpander-2022" },
                });
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

            migrationBuilder.DeleteData(table: "CarImages", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240 });
            migrationBuilder.DeleteData(table: "Cars", keyColumn: "Id", keyValues: new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241 });
            migrationBuilder.DeleteData(table: "CarModels", keyColumn: "Id", keyValues: new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97 });
            migrationBuilder.DeleteData(table: "CarBrands", keyColumn: "Id", keyValues: new object[] { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 });
        }
    }
}