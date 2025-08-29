using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Provides functionality to generate JWT access and refresh tokens for a user.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a JWT access token and a refresh token for the given user.
        /// </summary>
        /// <param name="user">The user information used in the token claims.</param>
        /// <returns>An <see cref="AuthenticationDto"/> containing the access token, refresh token, and expiration times.</returns>
        AuthenticationDto CreateJwtToken(JwtUserDto user);
    }
}
