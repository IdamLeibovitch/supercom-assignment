using Backend.Models;
using Backend.Data.Repositories;
using Backend.Services;
using FluentAssertions;
using Moq;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;
using Backend.Middleware;

namespace Backend.Tests.Services
{
  public class TasksServiceTests
  {
    private readonly Mock<ITasksRepository> _mockRepository;
    private readonly TasksService _service;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IHubContext<TasksHub>> _mockHubContext;
    private readonly Mock<ILogService> _mockLogService;
    private readonly Guid _testUserId;

    public TasksServiceTests()
    {
      _mockMapper = new Mock<IMapper>();
      _mockHubContext = new Mock<IHubContext<TasksHub>>();
      var mockClients = new Mock<IHubClients>();
      var mockClientProxy = new Mock<IClientProxy>();

      _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
      mockClients.Setup(x => x.All).Returns(mockClientProxy.Object);

      _mockLogService = new Mock<ILogService>();
      _mockRepository = new Mock<ITasksRepository>();
      _testUserId = Guid.NewGuid();

      _service = new TasksService(_mockRepository.Object, _mockHubContext.Object, _mockLogService.Object);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetTaskByIdAsync_WithExistingTask_ReturnsTask()
    {
      // Arrange
      var taskId = Guid.NewGuid();
      var expectedTask = new TaskInfo
      {
        Id = taskId,
        Title = "Test Task",
        Priority = TaskPriority.High,
        User = new UserInfo()
        {
          Id = _testUserId
        }
      };

      _mockRepository.Setup(x => x.GetTaskByIdAsync(taskId))
          .ReturnsAsync(expectedTask);

      // Act
      var result = await _service.GetTaskByIdAsync(taskId);

      // Assert
      result.Should().NotBeNull();
      result.Should().BeEquivalentTo(expectedTask);
    }

    [Fact]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithValidData_ReturnsTaskId()
    {
      // Arrange
      var title = "New Task";
      var description = "Description";
      var dueDate = DateTime.UtcNow.AddDays(7);
      var priority = TaskPriority.Medium;
      var expectedTaskId = Guid.NewGuid();

      _mockRepository.Setup(x => x.CreateTaskAsync(title, description, dueDate, priority, _testUserId))
          .ReturnsAsync(expectedTaskId);

      // Act
      var result = await _service.CreateTaskAsync(title, description, dueDate, priority, _testUserId);

      // Assert
      result.Should().Be(expectedTaskId);
      _mockRepository.Verify(x => x.CreateTaskAsync(title, description, dueDate, priority, _testUserId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task UpdateTaskAsync_WithValidData_UpdatesTask()
    {
      // Arrange
      var taskId = Guid.NewGuid();
      var title = "Updated Title";
      var description = "Updated Description";
      var dueDate = DateTime.UtcNow.AddDays(7);
      var priority = TaskPriority.High;

      _mockRepository.Setup(x => x.UpdateTaskAsync(taskId, title, description, dueDate, priority, _testUserId))
          .Returns(System.Threading.Tasks.Task.CompletedTask);

      // Act
      await _service.UpdateTaskAsync(taskId, title, description, dueDate, priority, _testUserId);

      // Assert
      _mockRepository.Verify(x => x.UpdateTaskAsync(taskId, title, description, dueDate, priority, _testUserId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task DeleteTaskAsync_WithExistingTask_DeletesTask()
    {
      // Arrange
      var taskId = Guid.NewGuid();
      _mockRepository.Setup(x => x.DeleteTaskAsync(taskId))
          .Returns(System.Threading.Tasks.Task.CompletedTask);

      // Act
      await _service.DeleteTaskAsync(taskId);

      // Assert
      _mockRepository.Verify(x => x.DeleteTaskAsync(taskId), Times.Once);
    }
    [Theory]
    [InlineData(TaskPriority.Low)]
    [InlineData(TaskPriority.Medium)]
    [InlineData(TaskPriority.High)]
    public async System.Threading.Tasks.Task CreateTaskAsync_WithDifferentPriorities_PassesCorrectPriority(TaskPriority priority)
    {
      // Arrange
      var title = "Task";
      var description = "Description";
      var dueDate = DateTime.UtcNow.AddDays(7);
      var expectedTaskId = Guid.NewGuid();

      _mockRepository.Setup(x => x.CreateTaskAsync(title, description, dueDate, priority, _testUserId))
          .ReturnsAsync(expectedTaskId);

      // Act
      await _service.CreateTaskAsync(title, description, dueDate, priority, _testUserId);

      // Assert
      _mockRepository.Verify(x => x.CreateTaskAsync(title, description, dueDate, priority, _testUserId), Times.Once);
    }

    [Fact]
    public async System.Threading.Tasks.Task GetAllTasksAsync_WithPagination_ReturnsPagedResults()
    {
      // Arrange
      var expectedTasks = new List<TaskInfo>
            {
                new TaskInfo() { Id = Guid.NewGuid(), Title = "Task 1", Priority = TaskPriority.Low, User = new UserInfo() { Id = _testUserId } },
                new TaskInfo() { Id = Guid.NewGuid(), Title = "Task 2", Priority = TaskPriority.High, User = new UserInfo() { Id = _testUserId } }
            };

      var page = 1;
      var pageSize = 10;
      var search = string.Empty;
      var sortBy = string.Empty;
      var ascending = true;
      var priorities = new List<TaskPriority>();
      var userIds = new List<Guid>();

      var expectedResult = new PaginatedResult<TaskInfo>
      {
        Items = expectedTasks,
        TotalCount = expectedTasks.Count,
        Page = page,
        PageSize = pageSize
      };

      _mockRepository.Setup(x => x.GetTasksAsync(page, pageSize, search, sortBy, ascending, priorities, userIds))
          .ReturnsAsync(expectedResult);

      // Act
      var result = await _service.GetTasksAsync(page, pageSize, search, sortBy, ascending, priorities, userIds);

      // Assert
      result.Items.Should().HaveCount(2);
      result.Should().BeEquivalentTo(expectedResult);
    }
  }
}
