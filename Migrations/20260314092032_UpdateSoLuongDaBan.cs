using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSoLuongDaBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SoLuongDaBan",
                table: "Saches",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 1,
                column: "SoLuongDaBan",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 2,
                column: "SoLuongDaBan",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoLuongDaBan",
                table: "Saches");
        }
    }
}
