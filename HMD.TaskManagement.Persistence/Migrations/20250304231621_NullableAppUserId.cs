using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HMD.TaskManagement.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class NullableAppUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_Priorities_PriorityId",
                table: "AppTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_AppTasks_Users_AppUserId",
                table: "AppTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskReports_AppTasks_AppTaskId",
                table: "TaskReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_AppRoles_AppRoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppTasks",
                table: "AppTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppRoles",
                table: "AppRoles");

            migrationBuilder.RenameTable(
                name: "AppTasks",
                newName: "Tasks");

            migrationBuilder.RenameTable(
                name: "AppRoles",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_AppTasks_PriorityId",
                table: "Tasks",
                newName: "IX_Tasks_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_AppTasks_AppUserId",
                table: "Tasks",
                newName: "IX_Tasks_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReports_Tasks_AppTaskId",
                table: "TaskReports",
                column: "AppTaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AppUserId",
                table: "Tasks",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_AppRoleId",
                table: "Users",
                column: "AppRoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskReports_Tasks_AppTaskId",
                table: "TaskReports");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Priorities_PriorityId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AppUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_AppRoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "AppTasks");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "AppRoles");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_PriorityId",
                table: "AppTasks",
                newName: "IX_AppTasks_PriorityId");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_AppUserId",
                table: "AppTasks",
                newName: "IX_AppTasks_AppUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppTasks",
                table: "AppTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppRoles",
                table: "AppRoles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_Priorities_PriorityId",
                table: "AppTasks",
                column: "PriorityId",
                principalTable: "Priorities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppTasks_Users_AppUserId",
                table: "AppTasks",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskReports_AppTasks_AppTaskId",
                table: "TaskReports",
                column: "AppTaskId",
                principalTable: "AppTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AppRoles_AppRoleId",
                table: "Users",
                column: "AppRoleId",
                principalTable: "AppRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
