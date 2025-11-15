using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBasicPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Privileges",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            var basicPrivileges = "TasksRead,TasksCreate,TasksWrite,TasksDelete,AllTasksRead";

            migrationBuilder.Sql(
                $"UPDATE Users SET Privileges = '{basicPrivileges}' WHERE Privileges IS NULL OR Privileges = ''");

            var allPrivileges = string.Join(",", new[]
            {
                "UsersRead",
                "UsersWrite",
                "UserPrivilegesRead",
                "UserPrivilegesWrite",
                "TasksRead",
                "TasksCreate",
                "TasksWrite",
                "TasksDelete",
                "AllTasksRead",
                "AllTasksCreate",
                "AllTasksWrite",
                "AllTasksDelete"
            });

            var adminUserId = Guid.NewGuid();
            migrationBuilder.Sql($@"
                IF NOT EXISTS (SELECT 1 FROM Users WHERE UserName = 'admin')
                BEGIN
                    INSERT INTO Users (Id, UserName, Password, FullName, Privileges)
                    VALUES ('{adminUserId}', 'admin', 'Admin123!', 'System Administrator', '{allPrivileges}')
                END
                ELSE
                BEGIN
                    UPDATE Users SET Privileges = '{allPrivileges}' WHERE UserName = 'admin'
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users WHERE UserName = 'admin'");

            migrationBuilder.DropColumn(
                name: "Privileges",
                table: "Users");
        }
    }
}
