using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class AddDriverLicenseBackImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriverLicenseImageUrl",
                table: "DriverDocuments",
                newName: "DriverLicenseFrontImageUrl");

            migrationBuilder.RenameColumn(
                name: "DriverLicenseImageUrl",
                table: "BookingDriverInfos",
                newName: "DriverLicenseFrontImageUrl");

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseBackImageUrl",
                table: "DriverDocuments",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverLicenseBackImageUrl",
                table: "BookingDriverInfos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverLicenseBackImageUrl",
                table: "DriverDocuments");

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
        }
    }
}
