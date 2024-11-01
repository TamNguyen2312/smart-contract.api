using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.FS
{
    /// <inheritdoc />
    public partial class addCoulumnToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfBirth",
                schema: "fs",
                table: "FS_Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdentityCard",
                schema: "fs",
                table: "FS_Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfBirth",
                schema: "fs",
                table: "FS_Users");

            migrationBuilder.DropColumn(
                name: "IdentityCard",
                schema: "fs",
                table: "FS_Users");
        }
    }
}
