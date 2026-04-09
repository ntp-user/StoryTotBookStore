using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class GioiTinh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GioiTinh",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                column: "GioiTinh",
                value: "Nam");

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                column: "GioiTinh",
                value: "Nam");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GioiTinh",
                table: "TaiKhoans");
        }
    }
}
