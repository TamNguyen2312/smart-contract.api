using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.App
{
    /// <inheritdoc />
    public partial class addActionColumnToSnapshotMetadataTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                schema: "app",
                table: "App_SnapshotMetadata",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                schema: "app",
                table: "App_SnapshotMetadata");
        }
    }
}
