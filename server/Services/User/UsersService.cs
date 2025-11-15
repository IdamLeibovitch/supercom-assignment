using AutoMapper;
using Backend.Data.Repositories;
using Backend.Middleware;
using Backend.Models;
using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Backend.Services;

/// <inheritdoc />
public class UsersService : IUsersService
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<TasksHub> _hubContext;
    private readonly ILogService _logService;

    public UsersService(
        IUsersRepository usersRepository,
        IMapper mapper,
        IHubContext<TasksHub> hubContext,
        ILogService logService)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _hubContext = hubContext;
        _logService = logService;
    }

    /// <inheritdoc />
    public async Task<PaginatedResult<UserInfo>> GetAllUsersAsync(int page = 1, int pageSize = 10)
    {
        return await _usersRepository.GetAllUsersAsync(page, pageSize);
    }

    /// <inheritdoc />
    public async Task<UserCredentials?> GetUserAsync(string username)
    {
        return await _usersRepository.GetUserAsync(username);
    }

    /// <inheritdoc />
    public async Task<UserInfo> GetUserByIdAsync(Guid userId)
    {
        var user = await _usersRepository.GetUserByIdAsync(userId)
            ?? throw new NotFoundException($"User with ID {userId} was not found.");
        return user!;
    }

    /// <inheritdoc />
    public async Task<UserDetails> GetUserDetailsAsync(Guid userId)
    {
        var userDetails = await _usersRepository.GetUserDetailsAsync(userId);

        if (userDetails == null)
        {
            throw new NotFoundException($"User with ID {userId} was not found.");
        }

        var privileges = await _usersRepository.GetUserPrivilegesAsync(userId);

        return userDetails;
    }

    public async Task<Guid> CreateUserAsync(string userName, string password)
    {
        var userId = await _usersRepository.CreateUserAsync(userName, password);

        await _logService.LogAsync<UserInfo>(AuditAction.Create, new { userId, userName });

        return userId;
    }

    /// <inheritdoc />
    public async Task UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber, string? email)
    {
        await _usersRepository.UpdateUserAsync(userId, fullName, phoneNumber, email);

        await _logService.LogAsync<UserInfo>(AuditAction.Update, new { userId, fullName, phoneNumber, email });

        await _hubContext.Clients.All.SendAsync(TasksHub.UserUpdated, userId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserPrivilege>> GetUserPrivilegesAsync(Guid userId)
    {
        return await _usersRepository.GetUserPrivilegesAsync(userId);
    }

    /// <inheritdoc />
    public async Task UpdateUserPrivilegesAsync(Guid userId, IEnumerable<UserPrivilege> privileges)
    {
        await _usersRepository.UpdateUserPrivilegesAsync(userId, privileges);

        await _logService.LogAsync<UserPrivilege>(AuditAction.Update, new { userId, privileges });
    }
}
