using System.Security.Claims;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;

public class FakeJwtService : IJwtService
{
    public AuthenticationDto CreateJwtToken(JwtUserDto user, bool reuseRefreshToken)
    {
        return new AuthenticationDto
        {
            AccessToken = "fake-access-token",
            AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
            RefreshToken = "fake-refresh-token",
            RefreshTokenExpiration = DateTime.UtcNow.AddDays(7)
        };
    }

    public ClaimsPrincipal? GetPrincipalFromJwtToken(string? token)
    {
        throw new NotImplementedException();
    }
}
