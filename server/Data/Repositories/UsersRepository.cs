using AutoMapper;
using AutoMapper.QueryableExtensions;
using Backend.Data.Models;
using Backend.Middleware;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data.Repositories
{
    /// <inheritdoc />
    public class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UsersRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserInfo>> GetAllUsersAsync(int page = 1, int pageSize = 10)
        {
            return await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<UserInfo>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<UserInfo?> GetUserByIdAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            return user == null ? null : _mapper.Map<UserInfo>(user);
        }

        /// <inheritdoc />
        public async Task<UserCredentials?> GetUserAsync(string username)
        {
            return await _context.Users
                .Where(u => u.UserName == username)
                .ProjectTo<UserCredentials>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<Guid> CreateUserAsync(string userName, string password)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == userName))
            {
                throw new ConflictException($"User with username '{userName}' already exists.");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = userName,
                Password = password,
                FullName = string.Empty,
                Tasks = []
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser.Id;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task UpdateUserAsync(Guid userId, string? fullName, string? phoneNumber, string? email)
        {
            var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException($"User with ID {userId} was not found.");
            user.FullName = fullName ?? string.Empty;
            user.PhoneNumber = phoneNumber;
            user.Email = email;

            await _context.SaveChangesAsync();
        }
    }
}