using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMD.TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeTaskAppUserIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Tasks",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Tasks] SET [AppUserId] = 1 WHERE [AppUserId] IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Tasks",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
