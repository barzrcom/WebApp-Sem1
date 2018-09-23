using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApp.Data.Migrations
{
	public partial class geoasfloat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<float>(
                name: "Longitude",
                table: "Location",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Location",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Longitude",
                table: "Location",
                nullable: false,
                oldClrType: typeof(float));

            migrationBuilder.AlterColumn<long>(
                name: "Latitude",
                table: "Location",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
