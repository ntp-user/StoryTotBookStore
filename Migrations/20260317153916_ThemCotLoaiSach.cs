using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotLoaiSach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LoaiSach",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 1,
                column: "LoaiSach",
                value: null);

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 2,
                column: "LoaiSach",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiSach",
                table: "Saches");
        }
    }
}
