using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class TaoBangDiaChi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiemTichLuy",
                table: "TaiKhoans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HangThanhVien",
                table: "TaiKhoans",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SoDiaChis",
                columns: table => new
                {
                    MaDiaChi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ten = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuocGia = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TinhThanhPho = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuanHuyen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    XaPhuong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiaChiCuThe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MaBuuDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LaMacDinh = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoDiaChis", x => x.MaDiaChi);
                });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                columns: new[] { "Avatar", "DiemTichLuy", "HangThanhVien" },
                values: new object[] { null, 0, "Thành viên Mới" });

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                columns: new[] { "Avatar", "DiemTichLuy", "HangThanhVien" },
                values: new object[] { null, 0, "Thành viên Mới" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoDiaChis");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "DiemTichLuy",
                table: "TaiKhoans");

            migrationBuilder.DropColumn(
                name: "HangThanhVien",
                table: "TaiKhoans");
        }
    }
}
