using Backend.Models;

namespace Backend.Data.Repositories
{
  public interface IUsersRepository
  {
    /// <summary>
    /// Retrieves all users
    /// </summary>
    /// <param name="page">The page number (1-based)</param>
    /// <param name="pageSize">The number of users per page</param>
    /// <returns>A list of all users</returns>
    Task<IEnumerable<UserInfo>> GetAllUsersAsync(int page = 1, int pageSize = 10);

    /// <summary>
    /// Retrieves a user by their username
    /// </summary>
    /// <param name="username">The username to search for</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<UserCredentials?> GetUserAsync(string username);

    /// <summary>
    /// Retrieves a user by their ID
    /// </summary>
    /// <param name="userId">The user ID to search for</param>
    /// <returns>The user if found, null otherwise</returns>
    Task<UserInfo?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Creates a new user with the specified username and password
    /// </summary>
    /// <param name="userName">The username for the new user</param>
    /// <param name="password">The password for the new user</param>
    /// <returns>The unique identifier (ID) of the newly created user</returns>
    Task<Guid> CreateUserAsync(string userName, string password);

    /// <summary>
    /// Updates an existing user with the specified username and password
    /// </summary>
    /// <param name="userId">The user ID of the user to update</param>
    /// <param name="fullName">The new full name for the user</param>
    /// <param name="phoneNumber">The new phone number for the user (optional)</param>
    /// <param name="email">The new email for the user (optional)</param>
    /// <returns>A task representing the asynchronous operation</returns>
    System.Threading.Tasks.Task UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber, string? email);
  }
}