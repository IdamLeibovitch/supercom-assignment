using Backend.Filters;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUsersService _usersService;

        public AuthController(IConfiguration configuration, IUsersService usersService)
        {
            _configuration = configuration;
            _usersService = usersService;
        }

        /// <summary>
        /// Attempt login
        /// </summary>
        [HttpPost("login")]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] UserCredentialsData data)
        {
            var user = await _usersService.GetUserAsync(data.UserName);

            if (user == null)
            {
                return Unauthorized();
            }

            if (user.Password != data.Password)
            {
                return Unauthorized();
            }

            var token = GenerateJwtToken(user.Id, user.UserName);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Sign up a new user
        /// </summary>
        [HttpPost("signup")]
        [ValidateModel]
        public async Task<IActionResult> SignUp([FromBody] UserCredentialsData data)
        {
            var existingUser = await _usersService.GetUserAsync(data.UserName);

            if (existingUser != null)
            {
                return BadRequest(new { message = "User already exists" });
            }

            var userId = await _usersService.CreateUserAsync(data.UserName, data.Password);

            var token = GenerateJwtToken(userId, data.UserName);
            return Ok(new { Token = token });
        }

        private string GenerateJwtToken(Guid userId, string userName)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, userName)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
