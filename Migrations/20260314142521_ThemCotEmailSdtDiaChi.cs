using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotEmailSdtDiaChi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "TaiKhoans",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TaiKhoans",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "TaiKhoans",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                columns: new[] { "DiaChi", "Email", "SoDienThoai" },
                values: new object[] { null, "admin@bookstore.com", "0988888888" });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                columns: new[] { "DiaChi", "Email", "SoDienThoai" },
                values: new object[] { null, "khachhang@gmail.com", "0909090909" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "TaiKhoans");
        }
    }
}
