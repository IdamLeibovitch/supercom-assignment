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
    public class UserControllerTests
    {
        private readonly Mock<IUsersService> _mockUsersService;
        private readonly UserController _controller;
        private readonly Guid _testUserId;
        private readonly string _testUserName;

        public UserControllerTests()
        {
            _mockUsersService = new Mock<IUsersService>();
            _controller = new UserController(_mockUsersService.Object);
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
        public async Task GetCurrentUser_WithValidUser_ReturnsOkWithUserInfo()
        {
            // Arrange
            var expectedUserInfo = new UserInfo
            {
                Id = _testUserId,
                UserName = _testUserName,
                FullName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "1234567890"
            };

            _mockUsersService
                .Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync(expectedUserInfo);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedUserInfo);
            _mockUsersService.Verify(s => s.GetUserByIdAsync(_testUserId), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUser_WithMissingUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _testUserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
            _mockUsersService.Verify(s => s.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetCurrentUser_WithInvalidUserIdFormat_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid"),
                new Claim(ClaimTypes.Name, _testUserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
            _mockUsersService.Verify(s => s.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task UpdateCurrentUser_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(_testUserId, userData.FullName, userData.PhoneNumber, userData.Email))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCurrentUser(userData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(_testUserId, userData.FullName, userData.PhoneNumber, userData.Email),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCurrentUser_WithNullValues_ReturnsNoContent()
        {
            // Arrange
            var userData = new UserData
            {
                FullName = null,
                Email = null,
                PhoneNumber = null
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(_testUserId, null, null, null))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateCurrentUser(userData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(_testUserId, null, null, null),
                Times.Once);
        }

        [Fact]
        public async Task UpdateCurrentUser_WithMissingUserIdClaim_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _testUserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            // Act
            var result = await _controller.UpdateCurrentUser(userData);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateCurrentUser_WithInvalidUserIdFormat_ReturnsUnauthorized()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid"),
                new Claim(ClaimTypes.Name, _testUserName)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            // Act
            var result = await _controller.UpdateCurrentUser(userData);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
            _mockUsersService.Verify(
                s => s.UpdateUserAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task UpdateCurrentUser_WhenServiceThrows_ThrowsException()
        {
            // Arrange
            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "9876543210"
            };

            _mockUsersService
                .Setup(s => s.UpdateUserAsync(_testUserId, userData.FullName, userData.PhoneNumber, userData.Email))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.UpdateCurrentUser(userData));
        }
    }
}
