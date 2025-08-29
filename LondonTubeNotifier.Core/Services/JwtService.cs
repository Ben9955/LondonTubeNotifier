using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace LondonTubeNotifier.Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }
        public AuthenticationDto CreateJwtToken(JwtUserDto user)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            Claim[] claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToString(), ClaimValueTypes.Integer64), 
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var tokenGenerator = new JwtSecurityToken( 
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials);

            var tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            string refreshToken = GenerateRefreshToken();
            DateTime refreshTokenExpiration = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);



            AuthenticationDto response = new AuthenticationDto
            {
                AccessToken = token,
                AccessTokenExpiration = expiration,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = refreshTokenExpiration
            };

            return response;
        }


        private string GenerateRefreshToken()
        {
            Byte[] bytes = new Byte[64];
            var randomGenerator = RandomNumberGenerator.Create();
            randomGenerator.GetBytes(bytes);
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }
    }
}
