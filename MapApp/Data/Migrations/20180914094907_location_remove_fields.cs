using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApp.Data.Migrations
{
	public partial class location_remove_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CloseTime",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "LastVisit",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "OpenTime",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Seasons",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Location");

            migrationBuilder.AddColumn<byte>(
                name: "Category",
                table: "Location",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Location",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Location");

            migrationBuilder.AddColumn<DateTime>(
                name: "CloseTime",
                table: "Location",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Location",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Duration",
                table: "Location",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastVisit",
                table: "Location",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "OpenTime",
                table: "Location",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte>(
                name: "Seasons",
                table: "Location",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "Type",
                table: "Location",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
