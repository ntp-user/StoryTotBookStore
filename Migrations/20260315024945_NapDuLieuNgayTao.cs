using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreManagement.Migrations
{
    /// <inheritdoc />
    public partial class NapDuLieuNgayTao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                column: "NgayTao",
                value: new DateTime(2026, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                column: "NgayTao",
                value: new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 1,
                column: "NgayTao",
                value: null);

            migrationBuilder.UpdateData(
                table: "TaiKhoans",
                keyColumn: "MaTK",
                keyValue: 2,
                column: "NgayTao",
                value: null);
        }
    }
}
