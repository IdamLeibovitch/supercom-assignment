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
        /// Get current authenticated user information with privileges
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<UserDetails>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var userDetails = await _usersService.GetUserDetailsAsync(userId);

            return Ok(userDetails);
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
