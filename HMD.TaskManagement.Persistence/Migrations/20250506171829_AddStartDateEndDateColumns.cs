using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMD.TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStartDateEndDateColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Tasks");
        }
    }
}
