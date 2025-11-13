using Backend.Filters;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UsersController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Get all users with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var users = await _usersService.GetAllUsersAsync(page, pageSize);
            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            return Ok(user);
        }

        [HttpPut("{userId}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserData data)
        {
            await _usersService.UpdateUserAsync(userId, data.FullName, data.PhoneNumber, data.Email);
            return NoContent();
        }
    }
}
