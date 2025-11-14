using Backend.Models;

namespace Backend.Data.Services;

public interface IUserPrivilegesCacheService
{
    /// <summary>
    /// Gets cached user privileges or retrieves them using the factory if not cached.
    /// </summary>
    Task<IEnumerable<UserPrivilege>> GetOrSetAsync(Guid userId, Func<Task<IEnumerable<UserPrivilege>>> factory);

    /// <summary>
    /// Invalidates cached privileges for the specified user.
    /// </summary>
    void Invalidate(Guid userId);
}
