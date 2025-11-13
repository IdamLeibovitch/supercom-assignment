using Backend.Controllers;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUsersService> _mockUsersService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUsersService = new Mock<IUsersService>();
            _controller = new UsersController(_mockUsersService.Object);
        }

        [Fact]
        public async Task GetUsers_WithDefaultPagination_ReturnsOkWithUsers()
        {
            // Arrange
            var expectedUsers = new List<UserInfo>
            {
                new UserInfo { Id = Guid.NewGuid(), UserName = "user1", FullName = "User One" },
                new UserInfo { Id = Guid.NewGuid(), UserName = "user2", FullName = "User Two" }
            };

            _mockUsersService
                .Setup(s => s.GetAllUsersAsync(1, 10))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedUsers);
            _mockUsersService.Verify(s => s.GetAllUsersAsync(1, 10), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WithCustomPagination_ReturnsOkWithUsers()
        {
            // Arrange
            var expectedUsers = new List<UserInfo>
            {
                new UserInfo { Id = Guid.NewGuid(), UserName = "user1", FullName = "User One" }
            };

            _mockUsersService
                .Setup(s => s.GetAllUsersAsync(2, 5))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.GetUsers(page: 2, pageSize: 5);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedUsers);
            _mockUsersService.Verify(s => s.GetAllUsersAsync(2, 5), Times.Once);
        }

        [Fact]
        public async Task GetUsers_WhenNoUsers_ReturnsEmptyList()
        {
            // Arrange
            _mockUsersService
                .Setup(s => s.GetAllUsersAsync(1, 10))
                .ReturnsAsync(new List<UserInfo>());

            // Act
            var result = await _controller.GetUsers();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var users = okResult!.Value as IEnumerable<UserInfo>;
            users.Should().BeEmpty();
        }

        [Fact]
        public async Task GetUser_WithValidId_ReturnsOkWithUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedUser = new UserInfo
            {
                Id = userId,
                UserName = "testuser",
                FullName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "1234567890"
            };

            _mockUsersService
                .Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedUser);
            _mockUsersService.Verify(s => s.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(userId, userData.FullName, userData.PhoneNumber, userData.Email))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(userId, userData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(userId, userData.FullName, userData.PhoneNumber, userData.Email),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WithNullValues_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userData = new UserData
            {
                FullName = null,
                Email = null,
                PhoneNumber = null
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(userId, null, null, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateUser(userId, userData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(userId, null, null, null),
                Times.Once);
        }

        [Fact]
        public async Task UpdateUser_WhenServiceThrows_ThrowsException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(userId, userData.FullName, userData.PhoneNumber, userData.Email))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateUser(userId, userData));
        }
    }
}
