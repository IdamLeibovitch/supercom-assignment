using Backend.Data.Services;
using Backend.Models;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Backend.Tests.Services
{
    public class UserPrivilegesCacheServiceTests
    {
        private readonly IMemoryCache _cache;
        private readonly UserPrivilegesCacheService _service;

        public UserPrivilegesCacheServiceTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new UserPrivilegesCacheService(_cache);
        }

        [Fact]
        public async Task GetOrSetAsync_WhenNotCached_CallsFactoryAndCachesResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var privileges = new[] { UserPrivilege.UsersRead, UserPrivilege.TasksRead };
            var factoryCallCount = 0;

            async Task<IEnumerable<UserPrivilege>> Factory()
            {
                factoryCallCount++;
                await Task.Delay(10);
                return privileges;
            }

            // Act
            var result1 = await _service.GetOrSetAsync(userId, Factory);
            var result2 = await _service.GetOrSetAsync(userId, Factory);

            // Assert
            result1.Should().BeEquivalentTo(privileges);
            result2.Should().BeEquivalentTo(privileges);
            factoryCallCount.Should().Be(1); // Factory should only be called once
        }

        [Fact]
        public async Task GetOrSetAsync_WhenCached_ReturnsFromCache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var privileges = new[] { UserPrivilege.UsersWrite, UserPrivilege.TasksWrite };

            async Task<IEnumerable<UserPrivilege>> Factory()
            {
                await Task.CompletedTask;
                return privileges;
            }

            await _service.GetOrSetAsync(userId, Factory);

            // Act
            var result = await _service.GetOrSetAsync(userId, async () =>
            {
                await Task.CompletedTask;
                return new[] { UserPrivilege.UsersDelete }; // Different value
            });

            // Assert
            result.Should().BeEquivalentTo(privileges); // Should return cached value
        }

        [Fact]
        public async Task Invalidate_RemovesFromCache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var initialPrivileges = new[] { UserPrivilege.UsersRead };
            var newPrivileges = new[] { UserPrivilege.UsersWrite };
            var factoryCallCount = 0;

            async Task<IEnumerable<UserPrivilege>> Factory()
            {
                await Task.CompletedTask;
                factoryCallCount++;
                return factoryCallCount == 1 ? initialPrivileges : newPrivileges;
            }

            // Act
            var result1 = await _service.GetOrSetAsync(userId, Factory);
            _service.Invalidate(userId);
            var result2 = await _service.GetOrSetAsync(userId, Factory);

            // Assert
            result1.Should().BeEquivalentTo(initialPrivileges);
            result2.Should().BeEquivalentTo(newPrivileges);
            factoryCallCount.Should().Be(2); // Factory called twice after invalidation
        }

        [Fact]
        public async Task GetOrSetAsync_WithEmptyResult_ReturnsEmptyEnumerable()
        {
            // Arrange
            var userId = Guid.NewGuid();

            async Task<IEnumerable<UserPrivilege>> Factory()
            {
                await Task.CompletedTask;
                return Enumerable.Empty<UserPrivilege>();
            }

            // Act
            var result = await _service.GetOrSetAsync(userId, Factory);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetOrSetAsync_WithDifferentUsers_CachesSeparately()
        {
            // Arrange
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var privileges1 = new[] { UserPrivilege.UsersRead };
            var privileges2 = new[] { UserPrivilege.TasksRead };

            // Act
            var result1 = await _service.GetOrSetAsync(userId1, async () =>
            {
                await Task.CompletedTask;
                return privileges1;
            });
            var result2 = await _service.GetOrSetAsync(userId2, async () =>
            {
                await Task.CompletedTask;
                return privileges2;
            });

            // Assert
            result1.Should().BeEquivalentTo(privileges1);
            result2.Should().BeEquivalentTo(privileges2);
        }
    }
}
