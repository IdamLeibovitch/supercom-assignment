using Backend.Middleware;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Backend.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequirePrivilegesAttribute : Attribute, IAuthorizationFilter
{
    private readonly UserPrivilege[] _requiredPrivileges;
    private readonly bool _requireAll;

    public RequirePrivilegesAttribute(params UserPrivilege[] privileges)
    {
        _requiredPrivileges = privileges;
        _requireAll = false;
    }
    
    public RequirePrivilegesAttribute(bool requireAll, params UserPrivilege[] privileges)
    {
        _requiredPrivileges = privileges;
        _requireAll = requireAll;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var userContext = context.HttpContext.GetAuthContext();
        
        if (userContext == null)
        {
            throw new ForbiddenException("User is not authenticated.");
        }

        bool hasAccess = _requireAll
            ? userContext.HasAllUserPrivileges(_requiredPrivileges)
            : userContext.HasAnyUserPrivilege(_requiredPrivileges);

        if (!hasAccess)
        {
            var privilegesNeeded = string.Join(", ", _requiredPrivileges.Select(p => p.ToString()));
            throw new ForbiddenException($"Missing required privilege(s): {privilegesNeeded}");
        }
    }
}
