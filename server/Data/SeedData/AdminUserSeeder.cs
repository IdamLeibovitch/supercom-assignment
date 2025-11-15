using Backend.Data.Models;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.SeedData;

public static class AdminUserSeeder
{
    public static async System.Threading.Tasks.Task SeedAdminUser(ApplicationDbContext context)
    {
        // Check if admin user already exists
        var adminExists = await context.Users.AnyAsync(u => u.UserName == "admin");

        if (!adminExists)
        {
            var allPrivileges = string.Join(",", Enum.GetValues<UserPrivilege>().Select(p => p.ToString()));

            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = "admin",
                Password = "Admin123!", // Note: This should be hashed in production
                FullName = "System Administrator",
                Email = "admin@system.local",
                Privileges = allPrivileges,
                Tasks = new List<Backend.Data.Models.Task>()
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }

    public static async System.Threading.Tasks.Task SeedBasicUsers(ApplicationDbContext context)
    {
        // Ensure all existing users have at least basic privileges
        var usersWithoutPrivileges = await context.Users
            .Where(u => string.IsNullOrEmpty(u.Privileges))
            .ToListAsync();

        if (usersWithoutPrivileges.Any())
        {
            var basicPrivileges = string.Join(",", new[]
            {
                UserPrivilege.TasksRead,
                UserPrivilege.TasksCreate,
                UserPrivilege.TasksWrite,
                UserPrivilege.TasksDelete
            }.Select(p => p.ToString()));

            foreach (var user in usersWithoutPrivileges)
            {
                user.Privileges = basicPrivileges;
            }

            await context.SaveChangesAsync();
        }
    }
}
