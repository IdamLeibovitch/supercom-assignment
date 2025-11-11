using AutoMapper;
using Backend.Data;
using Backend.Data.Models;
using Backend.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Backend.Tests.Mapping
{
  public class MappingProfileTests
  {
    private readonly IMapper _mapper;

    public MappingProfileTests()
    {
      var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
      _mapper = config.CreateMapper();
    }

    [Fact]
    public void MappingProfile_Configuration_IsValid()
    {
      // Assert
      var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>(), NullLoggerFactory.Instance);
      config.AssertConfigurationIsValid();
    }

    [Fact]
    public void Map_TaskToTaskInfo_MapsAllPropertiesCorrectly()
    {
      // Arrange
      var user = new User
      {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        FullName = "Test User",
        Email = "test@example.com",
        Password = "password",
        Tasks = new List<Backend.Data.Models.Task>()
      };

      var task = new Backend.Data.Models.Task
      {
        Id = Guid.NewGuid(),
        Title = "Test Task",
        Description = "Test Description",
        Priority = Backend.Data.Models.TaskPriority.High,
        DueDate = DateTime.UtcNow.AddDays(7),
        UserId = user.Id,
        User = user
      };

      // Act
      var result = _mapper.Map<TaskInfo>(task);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(task.Id);
      result.Title.Should().Be(task.Title);
      result.Description.Should().Be(task.Description);
      result.Priority.ToString().Should().Be(task.Priority.ToString());
      result.DueDate.Should().Be(task.DueDate);
      result.User.Should().NotBeNull();
      result.User.Id.Should().Be(user.Id);
    }

    [Theory]
    [InlineData(Data.Models.TaskPriority.Low)]
    [InlineData(Data.Models.TaskPriority.Medium)]
    [InlineData(Data.Models.TaskPriority.High)]
    public void Map_TaskToTaskInfo_PriorityMapsCorrectly(Data.Models.TaskPriority priority)
    {
      // Arrange
      var user = new User
      {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        FullName = "Test User",
        Password = "password",
        Tasks = new List<Backend.Data.Models.Task>()
      };

      var task = new Backend.Data.Models.Task
      {
        Id = Guid.NewGuid(),
        Title = "Test Task",
        Description = "Description",
        Priority = priority,
        DueDate = DateTime.UtcNow.AddDays(7),
        UserId = user.Id,
        User = user
      };

      // Act
      var result = _mapper.Map<TaskInfo>(task);

      // Assert
      result.Priority.ToString().Should().Be(task.Priority.ToString());
    }

    [Fact]
    public void Map_UserToUserInfo_MapsAllPropertiesCorrectly()
    {
      // Arrange
      var user = new User
      {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        FullName = "Test User",
        Email = "test@example.com",
        PhoneNumber = "1234567890",
        Password = "password",
        Tasks = new List<Backend.Data.Models.Task>()
      };

      // Act
      var result = _mapper.Map<UserInfo>(user);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(user.Id);
      result.FullName.Should().Be(user.FullName);
      result.Email.Should().Be(user.Email);
      result.PhoneNumber.Should().Be(user.PhoneNumber);
    }

    [Fact]
    public void Map_UserToUserCredentials_MapsUsernameAndPassword()
    {
      // Arrange
      var user = new User
      {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        Password = "hashedpassword",
        FullName = "Test User",
        Tasks = new List<Backend.Data.Models.Task>()
      };

      // Act
      var result = _mapper.Map<UserCredentials>(user);

      // Assert
      result.Should().NotBeNull();
      result.Id.Should().Be(user.Id);
      result.UserName.Should().Be(user.UserName);
      result.Password.Should().Be(user.Password);
    }

    [Fact]
    public void Map_TaskWithNullDescription_HandlesGracefully()
    {
      // Arrange
      var user = new User
      {
        Id = Guid.NewGuid(),
        UserName = "testuser",
        FullName = "Test User",
        Password = "password",
        Tasks = new List<Backend.Data.Models.Task>()
      };

      var task = new Backend.Data.Models.Task
      {
        Id = Guid.NewGuid(),
        Title = "Test Task",
        Description = null!,
        Priority = Data.Models.TaskPriority.Low,
        DueDate = DateTime.UtcNow.AddDays(7),
        UserId = user.Id,
        User = user
      };

      // Act
      var result = _mapper.Map<TaskInfo>(task);

      // Assert
      result.Should().NotBeNull();
      result.Description.Should().BeNull();
    }
  }
}
