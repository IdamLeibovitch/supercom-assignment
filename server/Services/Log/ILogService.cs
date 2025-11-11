namespace Backend.Services;

public interface ILogService
{
    /// <summary>
    /// Logs an audit action and changes asynchronously.
    /// </summary>
    /// <typeparam name="T">The type of the entity being logged.</typeparam>
    /// <param name="action">The audit action being performed.</param>
    /// <param name="changes">The object representing the changes to be logged.</param>
    /// <returns>A task that represents the asynchronous logging operation.</returns>
    Task LogAsync<T>(AuditAction action, object changes);
}
