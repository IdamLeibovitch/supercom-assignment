using Backend.Data;
using Backend.Models;
using Backend.Data.Repositories;
using Backend.Data.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Backend.Middleware;

namespace Backend.Tests.Repositories
{
    public class TasksRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly TasksRepository _repository;
        private readonly IMapper _mapper;
        private readonly Guid _testUserId;

        public TasksRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Backend.Data.Models.Task, TaskInfo>()
                    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));
                cfg.CreateMap<User, UserInfo>();
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _repository = new TasksRepository(_context, _mapper);

            // Setup test user
            _testUserId = Guid.NewGuid();
            _context.Users.Add(new User
            {
                Id = _testUserId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Tasks = new List<Backend.Data.Models.Task>()
            });
            _context.SaveChanges();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_WithExistingTask_ReturnsTask()
        {
            // Arrange
            var task = new Backend.Data.Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Test Task",
                Description = "Test Description",
                Priority = Data.Models.TaskPriority.High,
                UserId = _testUserId,
                User = _context.Users.Find(_testUserId)!
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTaskByIdAsync(task.Id);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(task.Id);
            result.Title.Should().Be(task.Title);
            result.Priority.ToString().Should().Be(task.Priority.ToString());
        }

        [Fact]
        public async System.Threading.Tasks.Task GetTaskByIdAsync_WithNonExistentTask_ReturnsNull()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _repository.GetTaskByIdAsync(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_WithValidData_AddsTaskToDatabase()
        {
            // Arrange
            var title = "New Task";
            var description = "Task Description";
            var priority = Models.TaskPriority.Medium;
            var dueDate = DateTime.UtcNow.AddDays(7);

            // Act
            var result = await _repository.CreateTaskAsync(title, description, dueDate, priority, _testUserId);

            // Assert
            result.Should().NotBeEmpty();
            var savedTask = await _context.Tasks.FindAsync(result);
            savedTask.Should().NotBeNull();
            savedTask!.Title.Should().Be(title);
            savedTask.Description.Should().Be(description);
            savedTask.Priority.ToString().Should().Be(priority.ToString());
            savedTask.UserId.Should().Be(_testUserId);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateTaskAsync_WithInvalidUserId_ThrowsNotFoundException()
        {
            // Arrange
            var invalidUserId = Guid.NewGuid();
            // Act
            Func<System.Threading.Tasks.Task> act = async () =>
                await _repository.CreateTaskAsync("Title", "Description", DateTime.UtcNow.AddDays(1), Models.TaskPriority.Low, invalidUserId);

            // Assert
            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_WithValidData_UpdatesTask()
        {
            // Arrange
            var task = new Backend.Data.Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Description = "Original Description",
                Priority = Data.Models.TaskPriority.Low,
                UserId = _testUserId,
                User = _context.Users.Find(_testUserId)!
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            var newTitle = "Updated Title";
            var newDescription = "Updated Description";
            var newPriority = Models.TaskPriority.High;
            var newDueDate = DateTime.UtcNow.AddDays(10);

            // Act
            await _repository.UpdateTaskAsync(task.Id, newTitle, newDescription, newDueDate, newPriority, _testUserId);

            // Assert
            var updatedTask = await _context.Tasks.FindAsync(task.Id);
            updatedTask!.Title.Should().Be(newTitle);
            updatedTask.Description.Should().Be(newDescription);
            updatedTask.Priority.ToString().Should().Be(newPriority.ToString());
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateTaskAsync_WithNonExistentTask_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            Func<System.Threading.Tasks.Task> act = async () =>
                await _repository.UpdateTaskAsync(nonExistentId, "Title", "Description", DateTime.UtcNow.AddDays(10), Models.TaskPriority.Low, _testUserId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_WithExistingTask_RemovesTask()
        {
            // Arrange
            var task = new Backend.Data.Models.Task
            {
                Id = Guid.NewGuid(),
                Title = "Task to Delete",
                Description = "Description",
                Priority = Data.Models.TaskPriority.Low,
                UserId = _testUserId,
                User = _context.Users.Find(_testUserId)!
            };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteTaskAsync(task.Id);

            // Assert
            var deletedTask = await _context.Tasks.FindAsync(task.Id);
            deletedTask.Should().BeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task DeleteTaskAsync_WithNonExistentTask_ThrowsNotFoundException()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _repository.DeleteTaskAsync(nonExistentId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetAllTasksAsync_WithPagination_ReturnsPagedResults()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.Tasks.Add(new Backend.Data.Models.Task
                {
                    Id = Guid.NewGuid(),
                    Title = $"Task {i}",
                    Description = "Description",
                    Priority = Data.Models.TaskPriority.Low,
                    UserId = _testUserId,
                    User = _context.Users.Find(_testUserId)!
                });
            }
            await _context.SaveChangesAsync();

            var page = 1;
            var pageSize = 10;
            var search = string.Empty;
            var sortBy = string.Empty;
            var ascending = true;
            var priorities = new List<Models.TaskPriority>();
            var userIds = new List<Guid>();

            // Act
            var result = await _repository.GetTasksAsync(page, pageSize, search, sortBy, ascending, priorities, userIds);

            // Assert
            result.Items.Should().HaveCount(10);
            result.TotalCount.Should().Be(15);
        }

        [Theory]
        [InlineData(Models.TaskPriority.Low)]
        [InlineData(Models.TaskPriority.Medium)]
        [InlineData(Models.TaskPriority.High)]
        public async System.Threading.Tasks.Task CreateTaskAsync_WithDifferentPriorities_MapsCorrectly(Models.TaskPriority priority)
        {
            // Act
            var taskId = await _repository.CreateTaskAsync("Title", "Description", DateTime.UtcNow.AddDays(1), priority, _testUserId);

            // Assert
            var task = await _context.Tasks.FindAsync(taskId);
            task.Should().NotBeNull();
            task!.Priority.ToString().Should().Be(priority.ToString());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
