using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.FS
{
    /// <inheritdoc />
    public partial class addGenderToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                schema: "fs",
                table: "FS_Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "fs",
                table: "FS_Users");
        }
    }
}
