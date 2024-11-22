using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.App
{
    /// <inheritdoc />
    public partial class updateContractTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppendixDaysLeft",
                schema: "app",
                table: "App_Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContractDaysLeft",
                schema: "app",
                table: "App_Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppendixDaysLeft",
                schema: "app",
                table: "App_Contracts");

            migrationBuilder.DropColumn(
                name: "ContractDaysLeft",
                schema: "app",
                table: "App_Contracts");
        }
    }
}
