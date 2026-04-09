using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class ThemCotMoTaSach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MoTa",
                table: "Saches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 1,
                columns: new[] { "GiaBan", "MoTa" },
                values: new object[] { 100000m, "Cuốn sách cung cấp kiến thức toàn diện về ASP.NET Core, từ cơ bản đến nâng cao, giúp bạn xây dựng website thực tế." });

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 2,
                column: "MoTa",
                value: "Cuốn sách cung cấp kiến thức toàn diện về ASP.NET Core, từ cơ bản đến nâng cao, giúp bạn xây dựng website thực tế.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoTa",
                table: "Saches");

            migrationBuilder.UpdateData(
                table: "Saches",
                keyColumn: "MaSach",
                keyValue: 1,
                column: "GiaBan",
                value: 150000m);
        }
    }
}
