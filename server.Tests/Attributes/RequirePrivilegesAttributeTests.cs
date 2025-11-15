using Backend.Attributes;
using Backend.Middleware;
using Backend.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Backend.Tests.Attributes
{
    public class RequirePrivilegesAttributeTests
    {
        [Fact]
        public void OnAuthorization_WithNoAuthContext_ThrowsForbiddenException()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UsersRead);
            var context = CreateAuthorizationFilterContext();

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().Throw<ForbiddenException>()
                .WithMessage("User is not authenticated.");
        }

        [Fact]
        public void OnAuthorization_WithRequiredPrivilege_DoesNotThrow()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UsersRead);
            var context = CreateAuthorizationFilterContext();
            context.HttpContext.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = [UserPrivilege.UsersRead, UserPrivilege.TasksRead]
            };

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void OnAuthorization_WithoutRequiredPrivilege_ThrowsForbiddenException()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UsersWrite);
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
                .WithMessage("Missing required privilege(s): UsersWrite");
        }

        [Fact]
        public void OnAuthorization_WithAnyRequiredPrivilege_DoesNotThrow()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(UserPrivilege.UsersRead, UserPrivilege.UsersWrite);
            var context = CreateAuthorizationFilterContext();
            context.HttpContext.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead }
            };

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void OnAuthorization_RequireAll_WithAllPrivileges_DoesNotThrow()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(true, UserPrivilege.UsersRead, UserPrivilege.TasksRead);
            var context = CreateAuthorizationFilterContext();
            context.HttpContext.Items["AuthContext"] = new UserContext
            {
                UserId = Guid.NewGuid(),
                Privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead, UserPrivilege.TasksWrite }
            };

            // Act
            Action act = () => attribute.OnAuthorization(context);

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void OnAuthorization_RequireAll_WithMissingPrivilege_ThrowsForbiddenException()
        {
            // Arrange
            var attribute = new RequirePrivilegesAttribute(true, UserPrivilege.UsersRead, UserPrivilege.UsersWrite);
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
                .WithMessage("Missing required privilege(s): UsersRead, UsersWrite");
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
