using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotHoTenKhachHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoTen",
                table: "TaiKhoans",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                column: "HoTen",
                value: "NTP");

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                column: "HoTen",
                value: "khachhang");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoTen",
                table: "TaiKhoans");
        }
    }
}
