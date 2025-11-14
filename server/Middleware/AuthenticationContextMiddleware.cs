using Backend.Data.Repositories;
using Backend.Models;
using System.Security.Claims;

namespace Backend.Middleware;

public class AuthenticationContext
{
    public Guid UserId { get; set; }
    public IEnumerable<UserPrivilege> Privileges { get; set; } = Enumerable.Empty<UserPrivilege>();

    public bool HasPrivilege(UserPrivilege privilege) => Privileges.Contains(privilege);
    public bool HasAnyPrivilege(params UserPrivilege[] privileges) => privileges.Any(HasPrivilege);
    public bool HasAllPrivileges(params UserPrivilege[] privileges) => privileges.All(HasPrivilege);
}

public class AuthenticationContextMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUsersRepository usersRepository)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userIdClaim, out var userId))
        {
            var privileges = await usersRepository.GetUserPrivilegesAsync(userId);

            var authContext = new AuthenticationContext
            {
                UserId = userId,
                Privileges = privileges
            };

            context.Items["AuthContext"] = authContext;
        }

        await _next(context);
    }
}

public static class AuthenticationContextExtensions
{
    public static AuthenticationContext? GetAuthContext(this HttpContext context)
    {
        return context.Items["AuthContext"] as AuthenticationContext;
    }
}
