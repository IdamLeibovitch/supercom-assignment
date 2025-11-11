using Backend.Models;

namespace Backend.Data.Repositories
{
    /// <summary>
    /// Represents a repository interface for managing tasks.
    /// </summary>
    public interface ITasksRepository
    {
        /// <summary>
        /// Retrieves a paginated list of tasks based on the specified filters and sorting options.
        /// </summary>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="pageSize">The number of tasks per page.</param>
        /// <param name="search">An optional search term to filter tasks by title or description.</param>
        /// <param name="sortBy">The field to sort the tasks by.</param>
        /// <param name="ascending">Indicates whether the sorting should be in ascending order.</param>
        /// <param name="priorities">An optional collection of task priorities to filter by.</param>
        /// <param name="userIds">An optional collection of user IDs to filter tasks by.</param>
        /// <returns>A paginated result containing the filtered and sorted tasks.</returns>
        Task<PaginatedResult<TaskInfo>> GetTasksAsync(
          int pageNumber,
          int pageSize,
          string? search,
          string? sortBy,
          bool ascending,
          IEnumerable<Backend.Models.TaskPriority>? priorities,
          IEnumerable<Guid>? userIds);

        /// <summary>
        /// Retrieves a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task.</param>
        /// <returns>The task information if found; otherwise, null.</returns>
        Task<TaskInfo> GetTaskByIdAsync(Guid id);

        /// <summary>
        /// Creates a new task with the specified details.
        /// </summary>
        /// <param name="title">The title of the task.</param>
        /// <param name="description">The description of the task.</param>
        /// <param name="dueDate">The due date of the task.</param>
        /// <param name="priority">The priority level of the task.</param>
        /// <param name="userId">The ID of the user assigned to the task.</param>
        /// <returns>The created task Id.</returns>
        Task<Guid> CreateTaskAsync(
          string title,
          string description,
          DateTime dueDate,
          TaskPriority priority,
          Guid userId);

        /// <summary>
        /// Updates an existing task with the specified details.
        /// </summary>
        /// <param name="id">The unique identifier of the task to update.</param>
        /// <param name="title">The updated title of the task.</param>
        /// <param name="description">The updated description of the task.</param>
        /// <param name="dueDate">The updated due date of the task.</param>
        /// <param name="priority">The updated priority level of the task.</param>
        /// <param name="userId">The updated user ID assigned to the task.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task UpdateTaskAsync(
          Guid id,
          string title,
          string description,
          DateTime dueDate,
          TaskPriority priority,
          Guid userId);

        /// <summary>
        /// Deletes a task by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the task to delete.</param>
        Task DeleteTaskAsync(Guid id);
    }
}
