using Backend.Models;

namespace Backend.Middleware;

public class UserContext
{
    private static IHttpContextAccessor? _httpContextAccessor;

    public static void Configure(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId { get; set; }
    public IEnumerable<UserPrivilege> Privileges { get; set; } = Enumerable.Empty<UserPrivilege>();

    public bool HasUserPrivilege(UserPrivilege privilege) => Privileges.Contains(privilege);
    public bool HasAnyUserPrivilege(params UserPrivilege[] privileges) => privileges.Any(this.HasUserPrivilege);
    public bool HasAllUserPrivileges(params UserPrivilege[] privileges) => privileges.All(this.HasUserPrivilege);

    public static UserContext? Current
    {
        get
        {
            var httpContext = _httpContextAccessor?.HttpContext;
            return httpContext?.Items["AuthContext"] as UserContext;
        }
    }

    public static bool HasPrivilege(UserPrivilege privilege)
    {
        return Current?.HasUserPrivilege(privilege) ?? false;
    }

    public static bool HasAnyPrivilege(params UserPrivilege[] privileges)
    {
        return Current?.HasAnyUserPrivilege(privileges) ?? false;
    }

    public static bool HasAllPrivileges(params UserPrivilege[] privileges)
    {
        return Current?.HasAllUserPrivileges(privileges) ?? false;
    }

    public static Guid? CurrentUserId => Current?.UserId;

    public static IEnumerable<UserPrivilege> CurrentUserPrivileges => Current?.Privileges ?? Enumerable.Empty<UserPrivilege>();
}
