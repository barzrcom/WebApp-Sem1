using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MapApp.Data.Migrations
{
    public partial class view_location_to_int : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "View",
                newName: "ID");

            migrationBuilder.AlterColumn<int>(
                name: "LocationId",
                table: "View",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ID",
                table: "View",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "LocationId",
                table: "View",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
