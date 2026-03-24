using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyBanHang.Migrations
{
    /// <inheritdoc />
    public partial class SuaTenCotLoaiSP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiSanXuatID",
                table: "SanPham");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiSanXuatID",
                table: "SanPham",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
