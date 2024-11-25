using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.App
{
    /// <inheritdoc />
    public partial class removeEndDateColumnInEmpContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "App_EmpContracts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "App_EmpContracts",
                type: "datetime2",
                nullable: true);
        }
    }
}
