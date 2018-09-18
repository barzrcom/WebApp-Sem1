using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApp.Data.Migrations
{
	public partial class remove_view_location : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "Location");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "Location",
                nullable: false,
                defaultValue: 0);
        }
    }
}
