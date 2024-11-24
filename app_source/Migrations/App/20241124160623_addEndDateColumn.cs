using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.App
{
    /// <inheritdoc />
    public partial class addEndDateColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "App_EmpContracts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "App_CustomerDeparmentAssigns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                schema: "app",
                table: "App_ContractDepartmentAssigns",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "App_EmpContracts");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "App_CustomerDeparmentAssigns");

            migrationBuilder.DropColumn(
                name: "EndDate",
                schema: "app",
                table: "App_ContractDepartmentAssigns");
        }
    }
}
