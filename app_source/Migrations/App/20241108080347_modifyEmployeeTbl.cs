using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations
{
    /// <inheritdoc />
    public partial class modifyEmployeeTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentName",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.AddColumn<long>(
                name: "DepartmentId",
                schema: "app",
                table: "App_Employees",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
            migrationBuilder.Sql("SET IDENTITY_INSERT [SmartContract].[app].[App_Employees] ON");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                schema: "app",
                table: "App_Employees");

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                schema: "app",
                table: "App_Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
