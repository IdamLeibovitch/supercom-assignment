using Backend.Data.Repositories;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;
using Backend.Middleware;
using Backend.Data.Models;

namespace Backend.Services
{
    /// <inheritdoc />
    public class TasksService : ITasksService
    {
        private readonly ITasksRepository _tasksRepository;
        private readonly IHubContext<TasksHub> _hubContext;
        private readonly ILogService _logService;

        public TasksService(ITasksRepository tasksRepository, IHubContext<TasksHub> hubContext, ILogService logService)
        {
            _tasksRepository = tasksRepository;
            _hubContext = hubContext;
            _logService = logService;
        }

        /// <inheritdoc />
        public async Task<PaginatedResult<TaskInfo>> GetTasksAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            bool ascending,
            IEnumerable<Backend.Models.TaskPriority>? priorities,
            IEnumerable<Guid>? userIds)
        {
            return await _tasksRepository.GetTasksAsync(pageNumber, pageSize, search, sortBy, ascending, priorities, userIds);
        }

        /// <inheritdoc />
        public async Task<TaskInfo> GetTaskByIdAsync(Guid id)
        {
            var canReadTasks = UserContext.HasPrivilege(UserPrivilege.TasksRead);
            var canReadAllTasks = UserContext.HasPrivilege(UserPrivilege.AllTasksRead);

            var task = await _tasksRepository.GetTaskByIdAsync(id);

            if (canReadTasks && !canReadAllTasks && task.User.Id != UserContext.CurrentUserId)
            {
                throw NotFoundException.For<Backend.Data.Models.Task>(id);
            }

            return task;
        }

        /// <inheritdoc />
        public async Task<Guid> CreateTaskAsync(
            string title,
            string description,
            DateTime dueDate,
            Backend.Models.TaskPriority priority,
            Guid userId)
        {
            var canReadTasks = UserContext.HasPrivilege(UserPrivilege.TasksRead);
            var canReadAllTasks = UserContext.HasPrivilege(UserPrivilege.AllTasksRead);

            if (canReadTasks && !canReadAllTasks && userId != UserContext.CurrentUserId)
            {
                throw new ForbiddenException("Cannot create tasks for other users");
            }

            var taskId = await _tasksRepository.CreateTaskAsync(title, description, dueDate, priority, userId);

            await _logService.LogAsync<TaskInfo>(AuditAction.Create, new { taskId, title, description, dueDate, priority, userId });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskCreated, taskId);

            return taskId;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task UpdateTaskAsync(
            Guid id,
            string title,
            string description,
            DateTime dueDate,
            Backend.Models.TaskPriority priority,
            Guid userId)
        {
            var canReadTasks = UserContext.HasPrivilege(UserPrivilege.TasksRead);
            var canReadAllTasks = UserContext.HasPrivilege(UserPrivilege.AllTasksRead);

            if (canReadTasks && !canReadAllTasks && userId != UserContext.CurrentUserId)
            {
                throw new ForbiddenException("Cannot edit tasks for other users");
            }

            await _tasksRepository.UpdateTaskAsync(id, title, description, dueDate, priority, userId);

            await _logService.LogAsync<TaskInfo>(AuditAction.Update, new { id, title, description, dueDate, priority, userId });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskUpdated, id);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTaskAsync(Guid userId)
        {
            var canReadTasks = UserContext.HasPrivilege(UserPrivilege.TasksRead);
            var canReadAllTasks = UserContext.HasPrivilege(UserPrivilege.AllTasksRead);

            if (canReadTasks && !canReadAllTasks && userId != UserContext.CurrentUserId)
            {
                throw new ForbiddenException("Cannot delete tasks for other users");
            }

            await _tasksRepository.DeleteTaskAsync(userId);

            await _logService.LogAsync<TaskInfo>(AuditAction.Delete, new { userId });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskDeleted, userId);
        }
    }
}
