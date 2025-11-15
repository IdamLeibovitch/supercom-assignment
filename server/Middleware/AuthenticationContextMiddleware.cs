using Backend.Data.Repositories;
using Backend.Models;
using System.Security.Claims;

namespace Backend.Middleware;

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

            var userContext = new UserContext
            {
                UserId = userId,
                Privileges = privileges
            };

            context.Items["AuthContext"] = userContext;
        }

        await _next(context);
    }
}

public static class AuthenticationContextExtensions
{
    public static UserContext? GetAuthContext(this HttpContext context)
    {
        return context.Items["AuthContext"] as UserContext;
    }

    public static bool HasPrivilege(this HttpContext context, UserPrivilege privilege)
    {
        var authContext = context.GetAuthContext();
        return authContext?.HasUserPrivilege(privilege) ?? false;
    }

    public static bool HasAnyPrivilege(this HttpContext context, params UserPrivilege[] privileges)
    {
        var authContext = context.GetAuthContext();
        return authContext?.HasAnyUserPrivilege(privileges) ?? false;
    }

    public static bool HasAllPrivileges(this HttpContext context, params UserPrivilege[] privileges)
    {
        var authContext = context.GetAuthContext();
        return authContext?.HasAllUserPrivileges(privileges) ?? false;
    }

    public static Guid? GetCurrentUserId(this HttpContext context)
    {
        var authContext = context.GetAuthContext();
        return authContext?.UserId;
    }

    public static IEnumerable<UserPrivilege> GetCurrentUserPrivileges(this HttpContext context)
    {
        var authContext = context.GetAuthContext();
        return authContext?.Privileges ?? Enumerable.Empty<UserPrivilege>();
    }
}
