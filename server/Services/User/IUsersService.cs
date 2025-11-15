using Backend.Models;

namespace Backend.Services;

/// <summary>
/// Service for managing user operations.
/// </summary>
public interface IUsersService
{
    /// <summary>
    /// Gets all users with pagination.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of users per page.</param>
    /// <returns>A paginated result of users.</returns>
    Task<PaginatedResult<UserInfo>> GetAllUsersAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Gets a user by their ID.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>The specified user.</returns>
    Task<UserInfo> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <returns>The user if found, otherwise null.</returns>
    Task<UserCredentials?> GetUserAsync(string username);

    /// <summary>
    /// Gets detailed user information by their ID.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <returns>The user details if found, otherwise null.</returns>
    Task<UserDetails> GetUserDetailsAsync(Guid userId);

    /// <summary>
    /// Creates a new user with the specified username and password.
    /// </summary>
    /// <param name="username">The username of the new user.</param>
    /// <param name="password">The password of the new user.</param>
    /// <returns>The created user's id.</returns>
    Task<Guid> CreateUserAsync(string username, string password);

    /// <summary>
    /// Updates an existing user with the specified user data.
    /// </summary>
    /// <param name="userId">The ID of the user to update.</param>
    /// <param name="fullName">The new full name for the user.</param>
    /// <param name="phoneNumber">The new phone number for the user (optional).</param>
    /// <param name="email">The new email for the user (optional).</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber, string? email);

    /// <summary>
    /// Retrieves user privileges by their ID
    /// </summary>
    /// <param name="userId">The user ID to search for</param>
    /// <returns>A list of user privileges</returns>
    Task<IEnumerable<UserPrivilege>> GetUserPrivilegesAsync(Guid userId);

    /// <summary>
    /// Updates user privileges
    /// </summary>
    /// <param name="userId">The user ID of the user to update</param>
    /// <param name="privileges">The new privileges for the user</param>
    /// <returns>A task representing the asynchronous operation</returns>
    Task UpdateUserPrivilegesAsync(Guid userId, IEnumerable<UserPrivilege> privileges);
}
