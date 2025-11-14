using Backend.Data;
using Backend.Models;
using Backend.Data.Repositories;
using Backend.Data.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Backend.Middleware;
using Xunit;
using AutoMapper;
using Moq;
using Backend.Data.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Backend.Tests.Repositories
{
    public class UsersRepositoryPrivilegesTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersRepository _repository;
        private readonly IMapper _mapper;
        private readonly Mock<IUserPrivilegesCacheService> _mockCacheService;

        public UsersRepositoryPrivilegesTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserCredentials>();
                cfg.CreateMap<User, UserInfo>();
                cfg.CreateMap<User, UserDetails>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                    .ForMember(dest => dest.Privileges, opt => opt.MapFrom(src =>
                        string.IsNullOrEmpty(src.Privileges)
                            ? null
                            : src.Privileges
                                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                                .Select(p => Enum.Parse<UserPrivilege>(p.Trim()))
                                .Distinct()
                                .ToList()));
            }, NullLoggerFactory.Instance);
            _mapper = config.CreateMapper();

            _mockCacheService = new Mock<IUserPrivilegesCacheService>();
            _mockCacheService
                .Setup(x => x.GetOrSetAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<IEnumerable<UserPrivilege>>>>()))
                .Returns<Guid, Func<Task<IEnumerable<UserPrivilege>>>>((_, factory) => factory());

            _repository = new UsersRepository(_context, _mapper, _mockCacheService.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserPrivilegesAsync_WithExistingUserAndPrivileges_ReturnsPrivileges()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Privileges = "UsersRead,TasksRead,TasksWrite",
                Tasks = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserPrivilegesAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(UserPrivilege.UsersRead);
            result.Should().Contain(UserPrivilege.TasksRead);
            result.Should().Contain(UserPrivilege.TasksWrite);
            _mockCacheService.Verify(x => x.GetOrSetAsync(userId, It.IsAny<Func<Task<IEnumerable<UserPrivilege>>>>()), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserPrivilegesAsync_WithExistingUserAndNoPrivileges_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Privileges = null,
                Tasks = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserPrivilegesAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserPrivilegesAsync_WithNonExistentUser_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _repository.GetUserPrivilegesAsync(userId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User {userId} not found.");
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateUserPrivilegesAsync_WithValidPrivileges_UpdatesUserAndInvalidatesCache()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Privileges = "UsersRead",
                Tasks = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var newPrivileges = new[] { UserPrivilege.UsersWrite, UserPrivilege.TasksRead, UserPrivilege.TasksDelete };

            // Act
            await _repository.UpdateUserPrivilegesAsync(userId, newPrivileges);

            // Assert
            var updatedUser = await _context.Users.FindAsync(userId);
            updatedUser.Should().NotBeNull();
            updatedUser!.Privileges.Should().Be("UsersWrite,TasksRead,TasksDelete");
            _mockCacheService.Verify(x => x.Invalidate(userId), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateUserPrivilegesAsync_WithEmptyPrivileges_SetsPrivilegesToNull()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Privileges = "UsersRead,TasksRead",
                Tasks = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _repository.UpdateUserPrivilegesAsync(userId, Enumerable.Empty<UserPrivilege>());

            // Assert
            var updatedUser = await _context.Users.FindAsync(userId);
            updatedUser.Should().NotBeNull();
            updatedUser!.Privileges.Should().BeNull();
            _mockCacheService.Verify(x => x.Invalidate(userId), Times.Once);
        }

        [Fact]
        public async System.Threading.Tasks.Task UpdateUserPrivilegesAsync_WithNonExistentUser_ThrowsNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var privileges = new[] { UserPrivilege.UsersRead };

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _repository.UpdateUserPrivilegesAsync(userId, privileges);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage($"User {userId} not found.");
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserDetailsByIdAsync_WithUserWithoutUserPrivilegesRead_ReturnsUserDetailsWithNullPrivileges()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Password = "password",
                FullName = "Test User",
                Email = "test@example.com",
                Privileges = "TasksRead,TasksWrite",
                Tasks = []
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserDetailsByIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.UserName.Should().Be("testuser");
            result.Email.Should().Be("test@example.com");
            result.Privileges.Should().NotBeNull();
            result.Privileges.Should().Contain(UserPrivilege.TasksRead);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
