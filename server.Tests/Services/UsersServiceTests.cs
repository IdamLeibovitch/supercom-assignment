using Backend.Models;
using Backend.Data.Repositories;
using Backend.Services;
using FluentAssertions;
using Moq;
using Xunit;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Backend.Hubs;

namespace Backend.Tests.Services
{
    public class UsersServiceTests
    {
        private readonly Mock<IUsersRepository> _mockRepository;
        private readonly UsersService _service;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IHubContext<TasksHub>> _mockHubContext;
        private readonly Mock<ILogService> _mockLogService;

        public UsersServiceTests()
        {
            _mockMapper = new Mock<IMapper>();
            _mockHubContext = new Mock<IHubContext<TasksHub>>();
            _mockLogService = new Mock<ILogService>();

            _mockRepository = new Mock<IUsersRepository>();
            _service = new UsersService(_mockRepository.Object, _mockMapper.Object, _mockHubContext.Object, _mockLogService.Object);
        }

        [Fact]
        public async Task GetUserAsync_WithExistingUsername_ReturnsUser()
        {
            // Arrange
            var userName = "testuser";
            var expectedUser = new UserCredentials { Id = Guid.NewGuid(), UserName = userName, Password = "password" };

            _mockRepository.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserAsync(userName);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUserAsync_WithNonExistentUsername_ReturnsNull()
        {
            // Arrange
            var userName = "nonexistent";

            _mockRepository.Setup(x => x.GetUserAsync(userName))
                .ReturnsAsync((UserCredentials?)null);

            // Act
            var result = await _service.GetUserAsync(userName);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateUserAsync_WithValidData_ReturnsUserId()
        {
            // Arrange
            var userName = "newuser";
            var password = "password123";
            var expectedUserId = Guid.NewGuid();

            _mockRepository.Setup(x => x.CreateUserAsync(userName, password))
                .ReturnsAsync(expectedUserId);

            // Act
            var result = await _service.CreateUserAsync(userName, password);

            // Assert
            result.Should().Be(expectedUserId);
            _mockRepository.Verify(
                x => x.CreateUserAsync(userName, password),
                Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_CallsRepositoryWithCorrectUserData()
        {
            // Arrange
            var userName = "newuser";
            var password = "password123";
            var expectedUserId = Guid.NewGuid();

            _mockRepository.Setup(x => x.CreateUserAsync(userName, password))
                .ReturnsAsync(expectedUserId);

            // Act
            var result = await _service.CreateUserAsync(userName, password);

            // Assert
            result.Should().Be(expectedUserId);
            _mockRepository.Verify(
                x => x.CreateUserAsync(userName, password),
                Times.Once);
        }
    }
}
