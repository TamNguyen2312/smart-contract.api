using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.FS
{
    /// <inheritdoc />
    public partial class addUserStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "fs",
                table: "FS_Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "fs",
                table: "FS_Users");
        }
    }
}
