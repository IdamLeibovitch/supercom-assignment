using Backend.Attributes;
using Backend.Controllers;
using Backend.Middleware;
using Backend.Models;
using Backend.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Backend.Tests.Controllers
{
    public class UsersControllerPrivilegesTests
    {
        private readonly Mock<IUsersService> _mockUsersService;
        private readonly UsersController _controller;

        public UsersControllerPrivilegesTests()
        {
            _mockUsersService = new Mock<IUsersService>();
            _controller = new UsersController(_mockUsersService.Object);
        }

        [Fact]
        public async Task GetUserPrivileges_WithUserPrivilegesRead_ReturnsPrivileges()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead };

            _mockUsersService
                .Setup(x => x.GetUserPrivilegesAsync(userId))
                .ReturnsAsync(privileges);

            _controller.ControllerContext = CreateControllerContext(currentUserId);

            // Act
            var result = await _controller.GetUserPrivileges(userId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var response = okResult!.Value as dynamic;
            ((IEnumerable<UserPrivilege>)response!.privileges).Should().BeEquivalentTo(privileges);
        }

        [Fact]
        public void GetUserPrivileges_WithoutUserPrivilegesRead_ThrowsForbiddenException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UserPrivilegesRead);
            var context = CreateAuthorizationFilterContext();
            context.HttpContext.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().Throw<ForbiddenException>()
                .WithMessage("Missing required privilege(s): UserPrivilegesRead");
        }

        [Fact]
        public async Task UpdateUserPrivileges_WithUserPrivilegesWrite_ReturnsNoContent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var request = new UpdatePrivilegesRequest
            {
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksWrite }
            };

            _mockUsersService
                .Setup(x => x.UpdateUserPrivilegesAsync(userId, request.Privileges))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = CreateControllerContext(currentUserId);

            // Act
            var result = await _controller.UpdateUserPrivileges(userId, request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(x => x.UpdateUserPrivilegesAsync(userId, request.Privileges), Times.Once);
        }

        [Fact]
        public void UpdateUserPrivileges_WithoutUserPrivilegesWrite_ThrowsForbiddenException()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UserPrivilegesWrite);
            var context = CreateAuthorizationFilterContext();
            context.HttpContext.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UserPrivilegesRead, UserPrivilege.TasksRead }
            };

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().Throw<ForbiddenException>()
                .WithMessage("Missing required privilege(s): UserPrivilegesWrite");
        }

        [Fact]
        public async Task UpdateUserPrivileges_WithEmptyPrivileges_UpdatesSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();
            var request = new UpdatePrivilegesRequest
            {
                Privileges = Enumerable.Empty<UserPrivilege>()
            };

            _mockUsersService
                .Setup(x => x.UpdateUserPrivilegesAsync(userId, request.Privileges))
                .Returns(Task.CompletedTask);

            _controller.ControllerContext = CreateControllerContext(currentUserId);

            // Act
            var result = await _controller.UpdateUserPrivileges(userId, request);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockUsersService.Verify(x => x.UpdateUserPrivilegesAsync(userId, request.Privileges), Times.Once);
        }

        private static ControllerContext CreateControllerContext(Guid userId)
        {
            return new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId.ToString())
                    }))
                }
            };
        }

        private static AuthorizationFilterContext CreateAuthorizationFilterContext()
        {
            var actionContext = new ActionContext(
                new DefaultHttpContext(),
                new RouteData(),
                new ActionDescriptor()
            );

            return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
        }
    }
}
