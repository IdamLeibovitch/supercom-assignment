namespace Backend.Data.Repositories;

public interface IAuditLogRepository
{
    /// <summary>
    /// Asynchronously adds an audit log entry with the specified entity name, action, and changes.
    /// </summary>
    /// <param name="entityName">The name of the entity being audited.</param>
    /// <param name="action">The action performed on the entity.</param>
    /// <param name="changes">Details of the changes made to the entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task AddAuditLogAsync(string entityName, string action, string changes);
}
