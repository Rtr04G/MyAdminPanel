using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorPanel.Migrations
{
    public partial class InitialCreate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileContent",
                table: "Documents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileContent",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
