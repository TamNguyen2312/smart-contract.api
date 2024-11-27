using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.API.Migrations.App
{
    /// <inheritdoc />
    public partial class addContractIdToContractAppendixTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ContractId",
                schema: "app",
                table: "App_ContractAppendices",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractId",
                schema: "app",
                table: "App_ContractAppendices");
        }
    }
}
