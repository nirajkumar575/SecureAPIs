using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SecureAPIs.Helpers;

namespace SecureAPIs.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtTokenService _jwtService;

        public AuthController(JwtTokenService jwtService)
        {
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Username == "admin" && request.Password == "password")
            {
                var token = _jwtService.GenerateToken(request.Username, "Admin");
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid credentials.");
        }

        [HttpGet("secure")]
        [EnableRateLimiting("fixed")]
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

}
