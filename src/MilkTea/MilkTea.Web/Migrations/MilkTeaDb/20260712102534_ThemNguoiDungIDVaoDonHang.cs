using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MilkTea.Web.Migrations.MilkTeaDb
{
    /// <inheritdoc />
    public partial class ThemNguoiDungIDVaoDonHang : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NguoiDungID",
                table: "DonHangs",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonHangs_NguoiDungID",
                table: "DonHangs",
                column: "NguoiDungID");

            migrationBuilder.AddForeignKey(
                name: "FK_DonHangs_AspNetUsers_NguoiDungID",
                table: "DonHangs",
                column: "NguoiDungID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DonHangs_AspNetUsers_NguoiDungID",
                table: "DonHangs");

            migrationBuilder.DropIndex(
                name: "IX_DonHangs_NguoiDungID",
                table: "DonHangs");

            migrationBuilder.DropColumn(
                name: "NguoiDungID",
                table: "DonHangs");
        }
    }
}
