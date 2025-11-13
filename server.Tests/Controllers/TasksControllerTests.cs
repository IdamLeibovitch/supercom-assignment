using Backend.Controllers;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class TasksControllerTests
    {
        private readonly Mock<ITasksService> _mockTasksService;
        private readonly TasksController _controller;
        private readonly Guid _testUserId;
        private readonly string _testUserName;

        public TasksControllerTests()
        {
            _mockTasksService = new Mock<ITasksService>();
            _controller = new TasksController(_mockTasksService.Object);
            _testUserId = Guid.NewGuid();
            _testUserName = "testuser";

            // Setup controller context with authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString()),
                new Claim(ClaimTypes.Name, _testUserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task GetTasks_WithDefaultParameters_ReturnsOkWithPaginatedResult()
        {
            // Arrange
            var expectedResult = new PaginatedResult<TaskInfo>
            {
                Items = new List<TaskInfo>
                {
                    new TaskInfo
                    {
                        Id = Guid.NewGuid(),
                        Title = "Task 1",
                        Description = "Description 1",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        Priority = TaskPriority.Medium,
                        User = new UserInfo { Id = _testUserId, UserName = _testUserName, FullName = "Test User" }
                    }
                },
                Page = 1,
                PageSize = 10,
                TotalCount = 1,
            };

            _mockTasksService
                .Setup(s => s.GetTasksAsync(1, 10, null, null, true, null, null))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTasks();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetTasks_WithSearchAndFilters_ReturnsFilteredResults()
        {
            // Arrange
            var priorities = new[] { TaskPriority.High, TaskPriority.Medium };
            var userIds = new[] { _testUserId };
            var expectedResult = new PaginatedResult<TaskInfo>
            {
                Items = new List<TaskInfo>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
            };

            _mockTasksService
                .Setup(s => s.GetTasksAsync(1, 10, "search", "priority", false, priorities, userIds))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTasks(
                pageNumber: 1,
                pageSize: 10,
                search: "search",
                sortBy: "priority",
                ascending: false,
                priorities: "High,Medium",
                users: _testUserId.ToString());

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            _mockTasksService.Verify(
                s => s.GetTasksAsync(1, 10, "search", "priority", false, It.IsAny<IEnumerable<TaskPriority>>(), It.IsAny<IEnumerable<Guid>>()),
                Times.Once);
        }

        [Fact]
        public async Task GetTasks_WithInvalidPriorityString_IgnoresInvalidValues()
        {
            // Arrange
            var expectedResult = new PaginatedResult<TaskInfo>
            {
                Items = new List<TaskInfo>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
            };

            _mockTasksService
                .Setup(s => s.GetTasksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<TaskPriority>>(),
                    It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTasks(priorities: "Invalid,High");

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task GetTask_WithValidId_ReturnsOkWithTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var expectedTask = new TaskInfo
            {
                Id = taskId,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(5),
                Priority = TaskPriority.High,
                User = new UserInfo { Id = _testUserId, UserName = _testUserName, FullName = "Test User" }
            };

            _mockTasksService
                .Setup(s => s.GetTaskByIdAsync(taskId))
                .ReturnsAsync(expectedTask);

            // Act
            var result = await _controller.GetTask(taskId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedTask);
        }

        [Fact]
        public async Task CreateTask_WithValidDataAndUserId_ReturnsCreatedAtAction()
        {
            // Arrange
            var taskData = new CreateTaskData
            {
                Title = "New Task",
                Description = "New Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = TaskPriority.Medium,
                UserId = _testUserId
            };
            var createdTaskId = Guid.NewGuid();

            _mockTasksService
                .Setup(s => s.CreateTaskAsync(taskData.Title, taskData.Description,
                    taskData.DueDate, taskData.Priority, _testUserId))
                .ReturnsAsync(createdTaskId);

            // Act
            var result = await _controller.CreateTask(taskData);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(_controller.GetTask));
            createdResult.RouteValues!["id"].Should().Be(createdTaskId);
            createdResult.Value.Should().Be(createdTaskId);
        }

        [Fact]
        public async Task CreateTask_WithoutUserId_UsesCurrentUser()
        {
            // Arrange
            var taskData = new CreateTaskData
            {
                Title = "New Task",
                Description = "New Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = TaskPriority.Medium,
                UserId = null
            };
            var createdTaskId = Guid.NewGuid();

            _mockTasksService
                .Setup(s => s.CreateTaskAsync(taskData.Title, taskData.Description,
                    taskData.DueDate, taskData.Priority, _testUserId))
                .ReturnsAsync(createdTaskId);

            // Act
            var result = await _controller.CreateTask(taskData);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            _mockTasksService.Verify(
                s => s.CreateTaskAsync(taskData.Title, taskData.Description,
                    taskData.DueDate, taskData.Priority, _testUserId),
                Times.Once);
        }

        [Fact]
        public async Task CreateTask_WithoutUserIdAndNoAuthenticatedUser_ReturnsBadRequest()
        {
            // Arrange
            var controller = new TasksController(_mockTasksService.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var taskData = new CreateTaskData
            {
                Title = "New Task",
                Description = "New Description",
                DueDate = DateTime.UtcNow.AddDays(7),
                Priority = TaskPriority.Medium,
                UserId = null
            };

            // Act
            var result = await controller.CreateTask(taskData);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            var badResult = result.Result as BadRequestObjectResult;
            badResult!.Value.Should().Be("User ID is required.");
        }

        [Fact]
        public async Task UpdateTask_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateData = new UpdateTaskData
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(10),
                Priority = TaskPriority.High,
                UserId = _testUserId
            };

            _mockTasksService
                .Setup(s => s.UpdateTaskAsync(taskId, updateData.Title, updateData.Description,
                    updateData.DueDate, updateData.Priority, updateData.UserId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateTask(taskId, updateData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockTasksService.Verify(
                s => s.UpdateTaskAsync(taskId, updateData.Title, updateData.Description,
                    updateData.DueDate, updateData.Priority, updateData.UserId),
                Times.Once);
        }

        [Fact]
        public async Task UpdateTask_WhenServiceThrows_ThrowsException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var updateData = new UpdateTaskData
            {
                Title = "Updated Task",
                Description = "Updated Description",
                DueDate = DateTime.UtcNow.AddDays(10),
                Priority = TaskPriority.High,
                UserId = _testUserId
            };

            _mockTasksService
                .Setup(s => s.UpdateTaskAsync(taskId, updateData.Title, updateData.Description,
                    updateData.DueDate, updateData.Priority, updateData.UserId))
                .ThrowsAsync(new Exception("Update failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateTask(taskId, updateData));
        }

        [Fact]
        public async Task DeleteTask_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockTasksService
                .Setup(s => s.DeleteTaskAsync(taskId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteTask(taskId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockTasksService.Verify(s => s.DeleteTaskAsync(taskId), Times.Once);
        }

        [Fact]
        public async Task DeleteTask_WhenServiceThrows_ThrowsException()
        {
            // Arrange
            var taskId = Guid.NewGuid();

            _mockTasksService
                .Setup(s => s.DeleteTaskAsync(taskId))
                .ThrowsAsync(new Exception("Delete failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.DeleteTask(taskId));
        }

        [Fact]
        public async Task GetTasks_WithEmptyPrioritiesString_PassesNullToService()
        {
            // Arrange
            var expectedResult = new PaginatedResult<TaskInfo>
            {
                Items = new List<TaskInfo>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
            };

            _mockTasksService
                .Setup(s => s.GetTasksAsync(1, 10, null, null, true, null, null))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTasks(priorities: "");

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            _mockTasksService.Verify(
                s => s.GetTasksAsync(1, 10, null, null, true, null, null),
                Times.Once);
        }

        [Fact]
        public async Task GetTasks_WithMultipleUserIds_ParsesCorrectly()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var expectedResult = new PaginatedResult<TaskInfo>
            {
                Items = new List<TaskInfo>(),
                TotalCount = 0,
                Page = 1,
                PageSize = 10,
            };

            _mockTasksService
                .Setup(s => s.GetTasksAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IEnumerable<TaskPriority>>(),
                    It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _controller.GetTasks(users: $"{userId1},{userId2}");

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            _mockTasksService.Verify(
                s => s.GetTasksAsync(1, 10, null, null, true, null, It.Is<IEnumerable<Guid>>(u => u.Count() == 2)),
                Times.Once);
        }
    }
}
