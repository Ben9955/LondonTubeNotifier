using AutoFixture;
using FluentAssertions;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Services;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UnitTests
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;
        private readonly JwtSettings _jwtSettings;
        private readonly Fixture _fixture;

        public JwtServiceTests()
        {
            _fixture = new Fixture();
            _jwtSettings = new JwtSettings
            {
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                SecretKey = "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef",
                ExpirationMinutes = 60,
                RefreshTokenExpirationDays = 7
            };

            var options = Options.Create(_jwtSettings);
            _jwtService = new JwtService(options);
        }

        [Fact]
        public void CreateJwtToken_ShouldReturnAuthenticationDto_WithValidAccessAndRefreshTokens()
        {
            // Arrange
            var user = new JwtUserDto
            {
                Id = Guid.NewGuid(),
                UserName = "testuser"
            };

            // Act
            var result = _jwtService.CreateJwtToken(user);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();

            result.AccessTokenExpiration.Should().BeAfter(DateTime.UtcNow);
            result.RefreshTokenExpiration.Should().BeAfter(DateTime.UtcNow);

            // Decode token to check claims
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result.AccessToken);

            token.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id.ToString());
            token.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == user.UserName);
            token.Issuer.Should().Be(_jwtSettings.Issuer);
            token.Audiences.Should().Contain(_jwtSettings.Audience);
        }

        [Fact]
        public void CreateJwtToken_ShouldSetCorrectExpirationTimes()
        {
            // Arrange
            var user = new JwtUserDto
            {
                Id = Guid.NewGuid(),
                UserName = "expiryTestUser"
            };

            var expectedAccessExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
            var expectedRefreshExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

            // Act
            var result = _jwtService.CreateJwtToken(user);

            // Assert
            result.AccessTokenExpiration.Should().BeCloseTo(expectedAccessExpiry, TimeSpan.FromSeconds(5));
            result.RefreshTokenExpiration.Should().BeCloseTo(expectedRefreshExpiry, TimeSpan.FromSeconds(5));
        }
    }
}
