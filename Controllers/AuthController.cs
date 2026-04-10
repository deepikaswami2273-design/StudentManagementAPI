using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace StudentManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            if (login == null)
            {
                return BadRequest("Invalid request");
            }

            if (login.Username == "admin" && login.Password == "1234")
            {
                var token = GenerateToken(login.Username);

                return Ok(new
                {
                    token = token,
                    message = "Login successful"
                });
            }

            return Unauthorized(new
            {
                message = "Invalid credentials"
            });
        }

        private string GenerateToken(string username)
        {
            // ✅ FIX: Strong key (must be 32+ characters)
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("ThisIsMySuperSecretKeyForJwtToken12345"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserLogin
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}