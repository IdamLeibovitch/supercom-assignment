using Backend.Extensions;
using Backend.Filters;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;

        public TasksController(ITasksService tasksService)
        {
            _tasksService = tasksService;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedResult<TaskInfo>>> GetTasks(
          [FromQuery] int pageNumber = 1,
          [FromQuery] int pageSize = 10,
          [FromQuery] string? search = null,
          [FromQuery] string? sortBy = null,
          [FromQuery] bool ascending = true,
          [FromQuery] string? priorities = null,
          [FromQuery] string? users = null)
        {
            IEnumerable<TaskPriority>? priorityList = null;
            IEnumerable<Guid>? userList = null;

            if (!string.IsNullOrEmpty(priorities))
            {
                priorityList = priorities
                    .Split(',')
                    .Select(p => Enum.TryParse<TaskPriority>(p, true, out var priority) ? priority : (TaskPriority?)null)
                    .Where(p => p.HasValue)
                    .Select(p => p!.Value);
            }

            if (!string.IsNullOrEmpty(users))
            {
                userList = users
                    .Split(',')
                    .Select(u => Guid.TryParse(u, out var user) ? user : (Guid?)null)
                    .Where(u => u.HasValue)
                    .Select(u => u!.Value);
            }

            var result = await _tasksService.GetTasksAsync(pageNumber, pageSize, search, sortBy, ascending, priorityList, userList);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TaskInfo>> GetTask(Guid id)
        {
            var task = await _tasksService.GetTaskByIdAsync(id);
            return Ok(task);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<ActionResult<Guid>> CreateTask([FromBody] CreateTaskData taskData)
        {
            Guid? userId = taskData.UserId;
            if (!userId.HasValue) userId = User.GetUserId();
            if (!userId.HasValue) return BadRequest("User ID is required.");

            var taskId = await _tasksService.CreateTaskAsync(
                taskData.Title,
                taskData.Description,
                taskData.DueDate,
                taskData.Priority,
                userId.Value);

            return CreatedAtAction(nameof(GetTask), new { id = taskId }, taskId);
        }

        [HttpPut("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskData taskData)
        {
            await _tasksService.UpdateTaskAsync(
                id,
                taskData.Title,
                taskData.Description,
                taskData.DueDate,
                taskData.Priority,
                taskData.UserId);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            await _tasksService.DeleteTaskAsync(id);
            return NoContent();
        }
    }
}
