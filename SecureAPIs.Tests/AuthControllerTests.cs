using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SecureAPIs.Controllers;
using SecureAPIs.Data;
using SecureAPIs.Helpers;
using SecureAPIs.Models;
using Xunit;

namespace SecureAPIs.Tests
{
    public class AuthControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IJwtTokenService> _jwtServiceMock;

        public AuthControllerTests()
        {
            // Use in-memory EF Core database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Seed test user
            _context.Users.Add(new User
            {
                Username = "testuser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("testpass"),
                Role = "Admin"
            });
            _context.SaveChanges();

            // Mock JWT token service
            _jwtServiceMock = new Mock<IJwtTokenService>();
            _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<string>(), It.IsAny<string>()))
                           .Returns("mock-jwt-token");
        }

        [Fact]
        public async Task Register_NewUser_ReturnsOk()
        {
            // Arrange
            var controller = new AuthController(_jwtServiceMock.Object, _context);
            var request = new RegisterRequest
            {
                Username = "newuser",
                Password = "newpass",
                Role = "User"
            };

            // Act
            var result = await controller.Register(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("User registered successfully.", okResult.Value);
        }

        [Fact]
        public async Task Register_ExistingUser_ReturnsBadRequest()
        {
            var controller = new AuthController(_jwtServiceMock.Object, _context);
            var request = new RegisterRequest
            {
                Username = "testuser",
                Password = "anypass",
                Role = "User"
            };

            var result = await controller.Register(request);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User already exists.", badRequest.Value);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsJwtToken()
        {
            var controller = new AuthController(_jwtServiceMock.Object, _context);
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "testpass"
            };

            var result = await controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var json = JsonConvert.SerializeObject(okResult.Value);
            var jObject = JObject.Parse(json);

            Assert.Equal("mock-jwt-token", jObject["Token"]?.ToString());

        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var controller = new AuthController(_jwtServiceMock.Object, _context);
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "wrongpass"
            };

            var result = await controller.Login(request);

            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid credentials.", unauthorizedResult.Value);
        }

        [Fact]
        public void SecureEndpoint_ReturnsOk()
        {
            // Act
            var controller = new AuthController(_jwtServiceMock.Object, _context);
            var result = controller.SecureEndpoint();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("You accessed a protected endpoint.", okResult.Value);
        }
    }
}
