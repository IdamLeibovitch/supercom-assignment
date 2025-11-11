using System.Security.Claims;

namespace Backend.Extensions
{
  public static class ClaimsPrincipalExtensions
  {
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
      var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      return Guid.TryParse(userId, out var guid) ? guid : (Guid?)null;
    }

    public static string? GetUserName(this ClaimsPrincipal principal)
    {
      return principal.FindFirst(ClaimTypes.Name)?.Value;
    }
  }
}
