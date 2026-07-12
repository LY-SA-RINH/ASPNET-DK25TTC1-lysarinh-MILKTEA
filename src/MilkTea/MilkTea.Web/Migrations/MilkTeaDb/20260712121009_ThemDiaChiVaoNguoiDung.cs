using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkTea.Web.Migrations.MilkTeaDb
{
    /// <inheritdoc />
    public partial class ThemDiaChiVaoNguoiDung : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiaChi",
                table: "AspNetUsers",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaChi",
                table: "AspNetUsers");
        }
    }
}
