using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApp.Data.Migrations
{
	public partial class locationuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "Location",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User",
                table: "Location");
        }
    }
}
