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
    public class UserControllerPrivilegesTests
    {
        private readonly Mock<IUsersService> _mockUsersService;
        private readonly UserController _controller;

        public UserControllerPrivilegesTests()
        {
            _mockUsersService = new Mock<IUsersService>();
            _controller = new UserController(_mockUsersService.Object);
        }

        [Fact]
        public async Task GetCurrentUser_WithValidUser_ReturnsUserInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userInfo = new UserInfo
            {
                Id = userId,
                UserName = "testuser",
                FullName = "Test User",
                Email = "test@example.com"
            };

            _mockUsersService
                .Setup(x => x.GetUserByIdAsync(userId))
                .ReturnsAsync(userInfo);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(userInfo);
        }

        [Fact]
        public async Task GetCurrentUser_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity())
                }
            };

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task UpdateCurrentUser_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userData = new UserData
            {
                FullName = "Updated Name",
                Email = "updated@example.com",
                PhoneNumber = "1234567890"
            };

            _mockUsersService
                .Setup(x => x.UpdateUserAsync(userId, userData.FullName, userData.PhoneNumber, userData.Email))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };

            // Act
            var result = await _controller.UpdateCurrentUser(userData);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(x => x.UpdateUserAsync(userId, userData.FullName, userData.PhoneNumber, userData.Email), Times.Once);
        }
    }
}
