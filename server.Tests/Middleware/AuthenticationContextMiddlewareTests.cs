using Backend.Data.Repositories;
using Backend.Middleware;
using Backend.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Backend.Tests.Middleware
{
    public class AuthenticationContextMiddlewareTests
    {
        private readonly Mock<IUsersRepository> _mockUsersRepository;
        private readonly Mock<RequestDelegate> _mockNext;
        private readonly AuthenticationContextMiddleware _middleware;

        public AuthenticationContextMiddlewareTests()
        {
            _mockUsersRepository = new Mock<IUsersRepository>();
            _mockNext = new Mock<RequestDelegate>();
            _middleware = new AuthenticationContextMiddleware(_mockNext.Object);
        }

        [Fact]
        public async Task InvokeAsync_WithAuthenticatedUser_AddsAuthContextToHttpContext()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead };

            _mockUsersRepository
                .Setup(x => x.GetUserPrivilegesAsync(userId))
                .ReturnsAsync(privileges);

            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            // Act
            await _middleware.InvokeAsync(context, _mockUsersRepository.Object);

            // Assert
            var authContext = context.GetAuthContext();
            authContext.Should().NotBeNull();
            authContext!.UserId.Should().Be(userId);
            authContext.Privileges.Should().BeEquivalentTo(privileges);
            _mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WithoutAuthenticatedUser_DoesNotAddAuthContext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            await _middleware.InvokeAsync(context, _mockUsersRepository.Object);

            // Assert
            var authContext = context.GetAuthContext();
            authContext.Should().BeNull();
            _mockUsersRepository.Verify(x => x.GetUserPrivilegesAsync(It.IsAny<Guid>()), Times.Never);
            _mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidUserId_DoesNotAddAuthContext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
            }));

            // Act
            await _middleware.InvokeAsync(context, _mockUsersRepository.Object);

            // Assert
            var authContext = context.GetAuthContext();
            authContext.Should().BeNull();
            _mockUsersRepository.Verify(x => x.GetUserPrivilegesAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public void AuthenticationContext_HasPrivilege_ReturnsTrueForExistingPrivilege()
        {
            // Arrange
            var authContext = new AuthenticationContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            authContext.HasPrivilege(UserPrivilege.UsersRead).Should().BeTrue();
            authContext.HasPrivilege(UserPrivilege.TasksRead).Should().BeTrue();
            authContext.HasPrivilege(UserPrivilege.UsersWrite).Should().BeFalse();
        }

        [Fact]
        public void AuthenticationContext_HasAnyPrivilege_ReturnsTrueIfAnyMatch()
        {
            // Arrange
            var authContext = new AuthenticationContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            authContext.HasAnyPrivilege(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeTrue();
            authContext.HasAnyPrivilege(UserPrivilege.UsersWrite, UserPrivilege.UsersDelete).Should().BeFalse();
        }

        [Fact]
        public void AuthenticationContext_HasAllPrivileges_ReturnsTrueIfAllMatch()
        {
            // Arrange
            var authContext = new AuthenticationContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead, UserPrivilege.TasksWrite }
            };

            // Act & Assert
            authContext.HasAllPrivileges(UserPrivilege.UsersRead, UserPrivilege.TasksRead).Should().BeTrue();
            authContext.HasAllPrivileges(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeFalse();
        }
    }
}
