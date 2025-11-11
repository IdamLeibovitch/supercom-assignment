using Backend.Data.Models;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace server.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Seed Users
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "UserName", "Password", "FullName", "PhoneNumber", "Email" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "john_doe", "Password123!", "John Doe", "+1234567890", "john.doe@example.com" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "jane_smith", "Password123!", "Jane Smith", "+1234567891", "jane.smith@example.com" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), "bob_johnson", "Password123!", "Bob Johnson", "+1234567892", "bob.johnson@example.com" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "alice_williams", "Password123!", "Alice Williams", "+1234567893", "alice.williams@example.com" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "charlie_brown", "Password123!", "Charlie Brown", "+1234567894", "charlie.brown@example.com" }
                });

            // Seed Tasks
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Title", "Description", "Priority", "DueDate", "UserId" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Complete project proposal", "Write and submit the Q1 project proposal with detailed budget breakdown", (int)TaskPriority.High, new DateTime(2024, 2, 1, 17, 0, 0), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Review code changes", "Review pull requests from team members for sprint 3", (int)TaskPriority.Medium, new DateTime(2024, 1, 25, 12, 0, 0), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), "Update documentation", "Update API documentation with new endpoints and examples", (int)TaskPriority.Low, new DateTime(2024, 1, 20, 15, 0, 0), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), "Fix bug #123", "Resolve the login authentication bug affecting mobile users", (int)TaskPriority.High, new DateTime(2024, 1, 24, 10, 0, 0), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"), "Deploy to staging", "Deploy latest changes to staging environment for QA testing", (int)TaskPriority.Medium, new DateTime(2024, 1, 26, 14, 0, 0), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "Database optimization", "Optimize database queries and add missing indexes", (int)TaskPriority.High, new DateTime(2024, 1, 30, 18, 0, 0), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("10101010-1010-1010-1010-101010101010"), "User training session", "Conduct training session for new features with the support team", (int)TaskPriority.Medium, new DateTime(2024, 1, 22, 13, 0, 0), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("20202020-2020-2020-2020-202020202020"), "Security audit", "Perform comprehensive security audit of the application", (int)TaskPriority.High, new DateTime(2024, 2, 5, 17, 0, 0), new Guid("44444444-4444-4444-4444-444444444444") },
                    { new Guid("30303030-3030-3030-3030-303030303030"), "Refactor payment module", "Refactor payment processing module to improve maintainability", (int)TaskPriority.Medium, new DateTime(2024, 1, 28, 12, 0, 0), new Guid("55555555-5555-5555-5555-555555555555") },
                    { new Guid("40404040-4040-4040-4040-404040404040"), "Create unit tests", "Write comprehensive unit tests for the new features", (int)TaskPriority.Low, new DateTime(2024, 2, 3, 16, 0, 0), new Guid("55555555-5555-5555-5555-555555555555") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seeded Tasks
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"),
                    new Guid("10101010-1010-1010-1010-101010101010"),
                    new Guid("20202020-2020-2020-2020-202020202020"),
                    new Guid("30303030-3030-3030-3030-303030303030"),
                    new Guid("40404040-4040-4040-4040-404040404040")
                });

            // Remove seeded Users
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValues: new object[]
                {
                    new Guid("11111111-1111-1111-1111-111111111111"),
                    new Guid("22222222-2222-2222-2222-222222222222"),
                    new Guid("33333333-3333-3333-3333-333333333333"),
                    new Guid("44444444-4444-4444-4444-444444444444"),
                    new Guid("55555555-5555-5555-5555-555555555555")
                });
        }
    }
}
