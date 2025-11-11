using Microsoft.AspNetCore.SignalR;

namespace Backend.Hubs;

public class TasksHub : Hub
{
    public const string TaskCreated = "TaskCreated";
    public const string TaskUpdated = "TaskUpdated";
    public const string TaskDeleted = "TaskDeleted";
    public const string UserUpdated = "UserUpdated";
}
