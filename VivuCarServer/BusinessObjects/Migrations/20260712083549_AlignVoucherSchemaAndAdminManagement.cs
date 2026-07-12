using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AlignVoucherSchemaAndAdminManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(name: "CreatedAt", table: "Vouchers", type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()");
            migrationBuilder.AddColumn<DateTime>(name: "ExpiresAt", table: "Vouchers", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<decimal>(name: "MaxDiscount", table: "Vouchers", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m);
            migrationBuilder.AddColumn<string>(name: "Name", table: "Vouchers", type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "");
            migrationBuilder.AddColumn<int>(name: "Quantity", table: "Vouchers", type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.Sql(@"
                UPDATE Vouchers SET
                    Name = COALESCE(NULLIF(Description, ''), Code),
                    ExpiresAt = EndDateTime,
                    MaxDiscount = COALESCE(MaxDiscountAmount, DiscountValue),
                    Quantity = COALESCE(UsageLimit, 0),
                    CreatedAt = StartDateTime,
                    DiscountType = CASE WHEN DiscountType = 'FixedAmount' THEN 'fixed' ELSE 'percentage' END;");

            migrationBuilder.DropColumn(name: "Description", table: "Vouchers");
            migrationBuilder.DropColumn(name: "EndDateTime", table: "Vouchers");
            migrationBuilder.DropColumn(name: "IsActive", table: "Vouchers");
            migrationBuilder.DropColumn(name: "MaxDiscountAmount", table: "Vouchers");
            migrationBuilder.DropColumn(name: "StartDateTime", table: "Vouchers");
            migrationBuilder.DropColumn(name: "UsageLimit", table: "Vouchers");
            migrationBuilder.DropColumn(name: "UsedCount", table: "Vouchers");

            migrationBuilder.AlterColumn<decimal>(name: "MinOrderAmount", table: "Vouchers", type: "decimal(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m, oldClrType: typeof(decimal), oldType: "decimal(18,2)", oldPrecision: 18, oldScale: 2, oldNullable: true);
            migrationBuilder.AlterColumn<string>(name: "DiscountType", table: "Vouchers", type: "nvarchar(20)", maxLength: 20, nullable: false, oldClrType: typeof(string), oldType: "nvarchar(32)", oldMaxLength: 32);

            migrationBuilder.UpdateData(table: "Vouchers", keyColumn: "Id", keyValue: 1,
                columns: new[] { "CreatedAt", "DiscountType", "ExpiresAt", "MaxDiscount", "Name", "Quantity" },
                values: new object[] { new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc), "percentage", new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc), 250000m, "VivuCar 10%", 100 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "MaxDiscount",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Vouchers");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Vouchers");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinOrderAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountType",
                table: "Vouchers",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Vouchers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Vouchers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "MaxDiscountAmount",
                table: "Vouchers",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateTime",
                table: "Vouchers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UsageLimit",
                table: "Vouchers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsedCount",
                table: "Vouchers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Vouchers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "DiscountType", "EndDateTime", "IsActive", "MaxDiscountAmount", "StartDateTime", "UsageLimit", "UsedCount" },
                values: new object[] { "Gi?m gi 10% t?ng ha don", "Percentage", new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, null, new DateTime(2026, 6, 1, 0, 0, 0, 0, DateTimeKind.Utc), 100, 0 });
        }
    }
}




