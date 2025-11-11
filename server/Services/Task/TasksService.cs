using Backend.Data.Repositories;
using Backend.Models;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

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
            IEnumerable<TaskPriority>? priorities,
            IEnumerable<Guid>? userIds)
        {
            return await _tasksRepository.GetTasksAsync(pageNumber, pageSize, search, sortBy, ascending, priorities, userIds);
        }

        /// <inheritdoc />
        public async Task<TaskInfo> GetTaskByIdAsync(Guid id)
        {
            return await _tasksRepository.GetTaskByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<Guid> CreateTaskAsync(
            string title,
            string description,
            DateTime dueDate,
            TaskPriority priority,
            Guid userId)
        {
            var taskId = await _tasksRepository.CreateTaskAsync(title, description, dueDate, priority, userId);

            await _logService.LogAsync<TaskInfo>(AuditAction.Create, new { taskId, title, description, dueDate, priority, userId });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskCreated, taskId);

            return taskId;
        }

        /// <inheritdoc />
        public async Task UpdateTaskAsync(
            Guid id,
            string title,
            string description,
            DateTime dueDate,
            TaskPriority priority,
            Guid userId)
        {
            await _tasksRepository.UpdateTaskAsync(id, title, description, dueDate, priority, userId);

            await _logService.LogAsync<TaskInfo>(AuditAction.Update, new { id, title, description, dueDate, priority, userId });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskUpdated, id);
        }

        /// <inheritdoc />
        public async Task DeleteTaskAsync(Guid id)
        {
            await _tasksRepository.DeleteTaskAsync(id);

            await _logService.LogAsync<TaskInfo>(AuditAction.Delete, new { id });

            await _hubContext.Clients.All.SendAsync(TasksHub.TaskDeleted, id);
        }
    }
}
