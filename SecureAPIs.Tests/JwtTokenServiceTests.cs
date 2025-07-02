using Microsoft.Extensions.Configuration;
using SecureAPIs.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Xunit;

namespace SecureAPIs.Tests
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public void GenerateToken_ShouldReturnValidJwtToken_WithCorrectClaims()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "ThisIsMyJwtSuperSecretKey123456789" },
                { "Jwt:Issuer", "TestIssuer" },
                { "Jwt:Audience", "TestAudience" },
                { "Jwt:ExpireMinutes", "60" }
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var jwtService = new JwtTokenService(configuration);

            string username = "testuser";
            string role = "Admin";

            // Act
            var token = jwtService.GenerateToken(username, role);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            var nameClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

            Assert.NotNull(nameClaim);
            Assert.Equal(username, nameClaim!.Value);

            Assert.NotNull(roleClaim);
            Assert.Equal(role, roleClaim!.Value);

            Assert.Equal("TestIssuer", jwt.Issuer);
            Assert.Equal("TestAudience", jwt.Audiences.First());
        }
    }
}
