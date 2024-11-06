using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations
{
    /// <inheritdoc />
    public partial class addCommonDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "app",
                table: "App_Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "app",
                table: "App_Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "app",
                table: "App_Employees",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "app",
                table: "App_Employees",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "app",
                table: "App_Employees",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "app",
                table: "App_Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                schema: "app",
                table: "App_Customers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                schema: "app",
                table: "App_Customers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                schema: "app",
                table: "App_Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "app",
                table: "App_Customers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "app",
                table: "App_Customers");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                schema: "app",
                table: "App_Customers");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                schema: "app",
                table: "App_Customers");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                schema: "app",
                table: "App_Customers");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "app",
                table: "App_Customers");
        }
    }
}
