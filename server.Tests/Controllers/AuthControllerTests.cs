using Backend.Controllers;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<IUsersService> _mockUsersService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUsersService = new Mock<IUsersService>();

            // Setup JWT configuration
            _mockConfiguration.Setup(x => x["Jwt:Key"]).Returns("ThisIsASecretKeyForTestingPurposesOnly123456");
            _mockConfiguration.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfiguration.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

            _controller = new AuthController(_mockConfiguration.Object, _mockUsersService.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "testuser", Password = "password123" };
            var user = new UserCredentials { Id = Guid.NewGuid(), UserName = "testuser", Password = "password123" };

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(credentials);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("Token")!.GetValue(okResult.Value).Should().NotBeNull();
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "nonexistent", Password = "password123" };

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync((UserCredentials?)null);

            // Act
            var result = await _controller.Login(credentials);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "testuser", Password = "wrongpassword" };
            var user = new UserCredentials { Id = Guid.NewGuid(), UserName = "testuser", Password = "correctpassword" };

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(credentials);

            // Assert
            result.Should().BeOfType<UnauthorizedResult>();
        }

        [Fact]
        public async Task SignUp_WithNewUser_ReturnsOkWithToken()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "newuser", Password = "password123" };
            var userId = Guid.NewGuid();

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync((UserCredentials?)null);
            _mockUsersService.Setup(x => x.CreateUserAsync(credentials.UserName, credentials.Password))
                .ReturnsAsync(userId);

            // Act
            var result = await _controller.SignUp(credentials);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().NotBeNull();
            okResult.Value!.GetType().GetProperty("Token")!.GetValue(okResult.Value).Should().NotBeNull();
        }

        [Fact]
        public async Task SignUp_WithExistingUser_ReturnsBadRequest()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "existinguser", Password = "password123" };
            var existingUser = new UserCredentials { Id = Guid.NewGuid(), UserName = "existinguser", Password = "oldpassword" };

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync(existingUser);

            // Act
            var result = await _controller.SignUp(credentials);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task SignUp_CallsCreateUserAsync_WithCorrectParameters()
        {
            // Arrange
            var credentials = new UserCredentialsData { UserName = "newuser", Password = "password123" };
            var userId = Guid.NewGuid();

            _mockUsersService.Setup(x => x.GetUserAsync(credentials.UserName))
                .ReturnsAsync((UserCredentials?)null);
            _mockUsersService.Setup(x => x.CreateUserAsync(credentials.UserName, credentials.Password))
                .ReturnsAsync(userId);

            // Act
            await _controller.SignUp(credentials);

            // Assert
            _mockUsersService.Verify(
                x => x.CreateUserAsync(credentials.UserName, credentials.Password),
                Times.Once);
        }
    }
}
