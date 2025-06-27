using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using SecureAPIs.Data;
using SecureAPIs.Helpers;
using System.Collections.Concurrent;

namespace SecureAPIs.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("fixed")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtService;
        private readonly ApplicationDbContext _context;
        private static readonly ConcurrentDictionary<string, (string Password, string Role)> _users = new();
        public AuthController(JwtTokenService jwtService,ApplicationDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            // 1. Check if username already exists in the database
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
                return BadRequest("User already exists.");

            // 2. Hash the password before saving
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Create User entity
            var newUser = new User
            {
                Username = request.Username,
                PasswordHash = hashedPassword,
                Role = request.Role ?? "User"
            };

            // 4. Save to database
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role);
            return Ok(new { Token = token });
        }

        [HttpGet("secure")]
        [Authorize(Roles = "Admin")]             
        public IActionResult SecureEndpoint()
        {
            return Ok("You accessed a protected endpoint.");
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class RegisterRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

}
