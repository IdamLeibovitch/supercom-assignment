using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/me")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public UserController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        /// <summary>
        /// Get current authenticated user information
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserInfo>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var userInfo = await _usersService.GetUserByIdAsync(userId);

            return Ok(userInfo);
        }

        /// <summary>
        /// Update current user information
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UserData data)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            await _usersService.UpdateUserAsync(userId, data.FullName, data.PhoneNumber, data.Email);
            return NoContent();
        }
    }
}
