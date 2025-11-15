using Backend.Attributes;
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
        [RequirePrivileges(UserPrivilege.UsersRead, UserPrivilege.AllTasksRead)]
        public async Task<ActionResult<PaginatedResult<UserInfo>>> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var paginatedUsers = await _usersService.GetAllUsersAsync(page, pageSize);
            return Ok(paginatedUsers);
        }

        /// <summary>
        /// Get a user by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePrivileges(UserPrivilege.UsersRead)]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var user = await _usersService.GetUserByIdAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Update a user's data
        /// </summary>
        [HttpPut("{userId}")]
        [ValidateModel]
        [RequirePrivileges(UserPrivilege.UsersWrite)]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserData data)
        {
            await _usersService.UpdateUserAsync(userId, data.FullName, data.PhoneNumber, data.Email);
            return NoContent();
        }

        /// <summary>
        /// Get user privileges by user ID
        /// </summary>
        [HttpGet("{id}/privileges")]
        [RequirePrivileges(UserPrivilege.UserPrivilegesRead)]
        public async Task<IActionResult> GetUserPrivileges(Guid id)
        {
            var privileges = await _usersService.GetUserPrivilegesAsync(id);
            return Ok(new { privileges });
        }

        /// <summary>
        /// Update user privileges
        /// </summary>
        [HttpPut("{id}/privileges")]
        [RequirePrivileges(UserPrivilege.UserPrivilegesWrite)]
        [ValidateModel]
        public async Task<IActionResult> UpdateUserPrivileges(Guid id, [FromBody] UpdatePrivilegesRequest request)
        {
            await _usersService.UpdateUserPrivilegesAsync(id, request.Privileges);
            return NoContent();
        }
    }
}
