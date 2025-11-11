using System.Text.Json;
using Backend.Data.Repositories;

namespace Backend.Services;

public class LogService : ILogService
{
    private readonly IAuditLogRepository _auditLogRepository;

    public LogService(IAuditLogRepository auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    /// <inheritdoc />
    public async Task LogAsync<T>(AuditAction action, object changes)
    {
        var entityName = typeof(T).Name;
        var actionString = action.ToString();
        var changesJson = JsonSerializer.Serialize(changes);

        await _auditLogRepository.AddAuditLogAsync(entityName, actionString, changesJson);
    }
}
