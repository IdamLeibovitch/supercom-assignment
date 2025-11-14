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
    public class UsersRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UsersRepository _repository;
        private readonly IMapper _mapper;
        private readonly Mock<IUserPrivilegesCacheService> _mockCacheService;

        public UsersRepositoryTests()
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

            // Setup default behavior for cache service
            _mockCacheService
                .Setup(x => x.GetOrSetAsync(It.IsAny<Guid>(), It.IsAny<Func<Task<IEnumerable<UserPrivilege>>>>()))
                .Returns<Guid, Func<Task<IEnumerable<UserPrivilege>>>>((_, factory) => factory());

            _repository = new UsersRepository(_context, _mapper, _mockCacheService.Object);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserAsync_WithExistingUser_ReturnsUser()
        {
            // Arrange
            var user = new User { Id = Guid.NewGuid(), UserName = "testuser", Password = "password", FullName = "", Tasks = Array.Empty<Data.Models.Task>() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetUserAsync("testuser");

            // Assert
            result.Should().NotBeNull();
            result!.UserName.Should().Be("testuser");
            result.Id.Should().Be(user.Id);
        }

        [Fact]
        public async System.Threading.Tasks.Task GetUserAsync_WithNonExistentUser_ReturnsNull()
        {
            // Act
            var result = await _repository.GetUserAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserAsync_WithValidUser_AddsUserToDatabase()
        {
            // Arrange
            var userName = "newuser";
            var password = "password";

            // Act
            var result = await _repository.CreateUserAsync(userName, password);

            // Assert
            result.Should().NotBeEmpty();
            var savedUser = await _context.Users.FindAsync(result);
            savedUser.Should().NotBeNull();
            savedUser!.UserName.Should().Be(userName);
            savedUser.Password.Should().Be(password);
        }

        [Fact]
        public async System.Threading.Tasks.Task CreateUserAsync_WithDuplicateUsername_ThrowsException()
        {
            // Arrange
            var userName = "duplicate";
            await _repository.CreateUserAsync(userName, "password1");

            // Act
            Func<System.Threading.Tasks.Task> act = async () => await _repository.CreateUserAsync(userName, "password2");

            // Assert
            await act.Should().ThrowAsync<ConflictException>();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
