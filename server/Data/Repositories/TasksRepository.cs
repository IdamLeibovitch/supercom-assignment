using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Data.Models;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories
{
    /// <inheritdoc />
    public class TasksRepository : ITasksRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TasksRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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
            var query = _context.Tasks.AsQueryable();

            // Filter by search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(t => t.Title.Contains(search) || t.Description.Contains(search));
            }

            // Filter by priorities
            if (priorities != null && priorities.Any())
            {
                var dalPriorities = priorities.Select(p => MappingProfile.MapEnum<Backend.Models.TaskPriority, Models.TaskPriority>(p)).ToList();
                query = query.Where(t => dalPriorities.Contains(t.Priority));
            }

            // Filter by users
            if (userIds != null && userIds.Any())
            {
                var missingUserIds = userIds.Where(userId => !_context.Users.Any(u => u.Id == userId)).ToList();
                if (missingUserIds.Any())
                {
                    foreach (var missingUserId in missingUserIds)
                    {
                        throw NotFoundException.For<User>(missingUserId);
                    }
                }

                query = query.Where(t => userIds.Contains(t.UserId));
            }

            // Sort
            query = sortBy?.ToLower() switch
            {
                "title" => ascending ? query.OrderBy(t => t.Title) : query.OrderByDescending(t => t.Title),
                "priority" => ascending ? query.OrderBy(t => t.Priority) : query.OrderByDescending(t => t.Priority),
                "username" => ascending ? query.OrderBy(t => t.User.FullName) : query.OrderByDescending(t => t.User.FullName),
                _ => ascending ? query.OrderBy(t => t.DueDate) : query.OrderByDescending(t => t.DueDate)
            };

            var totalCount = await query.CountAsync();

            var tasks = await query
              .Include(x => x.User)
              .Skip((pageNumber - 1) * pageSize)
              .Take(pageSize)
              .ProjectTo<TaskInfo>(_mapper.ConfigurationProvider)
              .ToListAsync();

            return new PaginatedResult<TaskInfo>
            {
                Items = tasks,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        /// <inheritdoc />
        public async Task<TaskInfo> GetTaskByIdAsync(Guid id)
        {
            var task = await _context.Tasks
                .Where(t => t.Id == id)
                .ProjectTo<TaskInfo>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync()
                ?? throw NotFoundException.For<Models.Task>(id);

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
            var user = await _context.Users.FindAsync(userId)
              ?? throw NotFoundException.For<User>(userId);

            var newTask = new Models.Task
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                DueDate = dueDate,
                Priority = MappingProfile.MapEnum<Backend.Models.TaskPriority, Models.TaskPriority>(priority),
                UserId = userId,
                User = user
            };

            _context.Tasks.Add(newTask);

            await _context.SaveChangesAsync();

            return newTask.Id;
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
            var task = await _context.Tasks.FindAsync(id)
              ?? throw NotFoundException.For<Models.Task>(id);

            task.Title = title;
            task.Description = description;
            task.DueDate = dueDate;
            task.Priority = MappingProfile.MapEnum<Backend.Models.TaskPriority, Models.TaskPriority>(priority);
            task.UserId = userId;

            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task DeleteTaskAsync(Guid id)
        {
            var task = await _context.Tasks.FindAsync(id)
              ?? throw NotFoundException.For<Models.Task>(id);

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();
        }
    }
}
