using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class SeedBookingAndPaymentTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CarBrands",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[] { 1, true, "Toyota" });

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
                columns: new[] { "Id", "CarBrandId", "CarModelId", "CreatedAt", "DailyPrice", "DeliveryFee", "DepositAmount", "Description", "FuelType", "InsuranceFeePerDay", "LicensePlate", "Location", "Name", "OwnerId", "SeatCount", "Status", "TransmissionType", "UpdatedAt" },
                values: new object[] { 1, 1, 1, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 600000m, 10000m, 1500000m, "Xe gia đình 5 chỗ sạch sẽ, vận hành êm ái, tiết kiệm nhiên liệu.", "Gasoline", 50000m, "43A-12345", "Hải Châu, Đà Nẵng", "Toyota Vios 2022", 6, 5, "Available", "Automatic", null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
