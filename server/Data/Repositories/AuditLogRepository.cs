using Backend.Data.Models;

namespace Backend.Data.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ApplicationDbContext _context;

    public AuditLogRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async System.Threading.Tasks.Task AddAuditLogAsync(string entityName, string action, string changes)
    {
        _context.AuditLogs.Add(new AuditLog
        {
            EntityName = entityName,
            Action = action,
            Changes = changes,
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }
}
