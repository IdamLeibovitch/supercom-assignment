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
            var userContext = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            userContext.HasUserPrivilege(UserPrivilege.UsersRead).Should().BeTrue();
            userContext.HasUserPrivilege(UserPrivilege.TasksRead).Should().BeTrue();
            userContext.HasUserPrivilege(UserPrivilege.UsersWrite).Should().BeFalse();
        }

        [Fact]
        public void AuthenticationContext_HasAnyPrivilege_ReturnsTrueIfAnyMatch()
        {
            // Arrange
            var userContext = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            userContext.HasAnyUserPrivilege(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeTrue();
        }

        [Fact]
        public void AuthenticationContext_HasAllPrivileges_ReturnsTrueIfAllMatch()
        {
            // Arrange
            var userContext = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead, UserPrivilege.TasksWrite }
            };

            // Act & Assert
            userContext.HasAllUserPrivileges(UserPrivilege.UsersRead, UserPrivilege.TasksRead).Should().BeTrue();
            userContext.HasAllUserPrivileges(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeFalse();
        }

        [Fact]
        public void HttpContextExtension_HasPrivilege_ReturnsTrueWhenUserHasPrivilege()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            context.HasPrivilege(UserPrivilege.UsersRead).Should().BeTrue();
            context.HasPrivilege(UserPrivilege.TasksRead).Should().BeTrue();
            context.HasPrivilege(UserPrivilege.UsersWrite).Should().BeFalse();
        }

        [Fact]
        public void HttpContextExtension_HasPrivilege_ReturnsFalseWhenNoAuthContext()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act & Assert
            context.HasPrivilege(UserPrivilege.UsersRead).Should().BeFalse();
        }

        [Fact]
        public void HttpContextExtension_HasAnyPrivilege_ReturnsTrueWhenUserHasAny()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act & Assert
            context.HasAnyPrivilege(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeTrue();
        }

        [Fact]
        public void HttpContextExtension_HasAllPrivileges_ReturnsTrueWhenUserHasAll()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead, UserPrivilege.TasksWrite }
            };

            // Act & Assert
            context.HasAllPrivileges(UserPrivilege.UsersRead, UserPrivilege.TasksRead).Should().BeTrue();
            context.HasAllPrivileges(UserPrivilege.UsersRead, UserPrivilege.UsersWrite).Should().BeFalse();
        }

        [Fact]
        public void HttpContextExtension_GetCurrentUserId_ReturnsUserIdWhenAuthenticated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var context = new DefaultHttpContext();
            context.Items["AuthContext"] = new UserContext
            {
                UserId = userId,
                Privileges = new[] { UserPrivilege.UsersRead }
            };

            // Act
            var result = context.GetCurrentUserId();

            // Assert
            result.Should().Be(userId);
        }

        [Fact]
        public void HttpContextExtension_GetCurrentUserId_ReturnsNullWhenNotAuthenticated()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var result = context.GetCurrentUserId();

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void HttpContextExtension_GetCurrentUserPrivileges_ReturnsPrivilegesWhenAuthenticated()
        {
            // Arrange
            var privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead };
            var context = new DefaultHttpContext();
            context.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = privileges
            };

            // Act
            var result = context.GetCurrentUserPrivileges();

            // Assert
            result.Should().BeEquivalentTo(privileges);
        }

        [Fact]
        public void HttpContextExtension_GetCurrentUserPrivileges_ReturnsEmptyWhenNotAuthenticated()
        {
            // Arrange
            var context = new DefaultHttpContext();

            // Act
            var result = context.GetCurrentUserPrivileges();

            // Assert
            result.Should().BeEmpty();
        }
    }
}
