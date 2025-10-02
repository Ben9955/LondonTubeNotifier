using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.WebApi.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace LondonTubeNotifier.WebApi.Controllers
{
    /// <summary>
    /// Handles user authentication: registration, login, and logout.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="request">Registration details.</param>
        /// <returns>Authentication data for the new user.</returns>
        /// <response code="200">User registered successfully.</response>
        /// <response code="400">Username or email already exists.</response>
        /// <remarks>Returns AccessToken and RefreshToken on success.</remarks>
        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponse>> Register([FromBody] RegisterRequest request)
        {

            if (await _userManager.FindByNameAsync(request.UserName) != null)
                return BadRequest(new { Error = "Username already taken" });

            if (await _userManager.FindByEmailAsync(request.Email) != null)
                return BadRequest(new { Error = "Email already registered" });

            var user = new ApplicationUser
            {
                UserName = request.UserName,
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                PushNotifications = request.PushNotifications,
                EmailNotifications = request.EmailNotifications,
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            var jwtUser = new JwtUserDto { Id = user.Id, UserName = user.UserName };
            var auth = _jwtService.CreateJwtToken(jwtUser, false);

            user.RefreshToken = auth.RefreshToken;
            user.RefreshTokenExpiration = auth.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            Response.Cookies.Append(
                "refreshToken",
                auth.RefreshToken!,
                GetCookieOptions(auth.RefreshTokenExpiration)
            );

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                PushNotifications = user.PushNotifications,
                EmailNotifications = user.EmailNotifications,
                AccessToken = auth.AccessToken,
                AccessTokenExpiration = auth.AccessTokenExpiration
            };

            return Ok(response);

        }

        /// <summary>
        /// Logs in a user with email/username and password.
        /// </summary>
        /// <param name="request">Login details.</param>
        /// <returns>Authentication data on success.</returns>
        /// <response code="200">Login successful.</response>
        /// <response code="401">Invalid credentials.</response>
        /// <remarks>Returns AccessToken and RefreshToken on success.</remarks>
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.EmailOrUsername)
                       ?? await _userManager.FindByEmailAsync(request.EmailOrUsername);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                return Unauthorized(new { Error = "Authentication failed" });

            var jwtUser = new JwtUserDto { Id = user.Id, UserName = user.UserName! };
            var auth = _jwtService.CreateJwtToken(jwtUser, false);

            user.RefreshToken = auth.RefreshToken;
            user.RefreshTokenExpiration = auth.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            Response.Cookies.Append(
                "refreshToken",
                auth.RefreshToken!,
                GetCookieOptions(auth.RefreshTokenExpiration)
            );

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                PushNotifications = user.PushNotifications,
                EmailNotifications = user.EmailNotifications,
                AccessToken = auth.AccessToken,
                AccessTokenExpiration = auth.AccessTokenExpiration
            };

            return Ok(response);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<ActionResult<UpdateProfileResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new { Error = "User not found" });
            }

            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;
            user.PushNotifications = request.PushNotifications;
            user.EmailNotifications = request.EmailNotifications;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            // Return fresh profile info
            var response = new UpdateProfileResponse
            {
                Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                PushNotifications = user.PushNotifications,
                EmailNotifications = user.EmailNotifications
            };

            return Ok(response);
        }


        /// <summary>
        /// Logs out the user by clearing their refresh token.
        /// </summary>
        /// <returns>No content on success.</returns>
        /// <response code="204">User logged out successfully.</response>
        /// <remarks>Clears refresh token from the database.</remarks>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            string? refreshToken = Request.Cookies["refreshToken"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiration = null;
                    await _userManager.UpdateAsync(user);
                }
            }

            Response.Cookies.Append("refreshToken", "", GetCookieOptions(DateTime.UtcNow.AddDays(-1)));
            return NoContent();
        }

        /// <summary>
        /// Generates a new access token using a valid refresh token.
        /// </summary>
        /// <param name="tokens">The current access token and refresh token pair.</param>
        /// <returns>A new <see cref="AuthenticationResponse"/> containing a refreshed access token.</returns>
        /// <response code="200">A new access token was successfully generated.</response>
        /// <response code="400">The provided tokens are invalid or the refresh token has expired.</response>
        [HttpPost("generate-new-jwt-token")]
        public async Task<ActionResult<AuthenticationResponse>> GenerateNewJwtToken()
        {
            string? refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest(new { Error = "No refresh token provided." });
            }

            var user = _userManager.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null)
            {
                return BadRequest(new { Error = "Invalid refresh token." });
            }

            if (user.RefreshTokenExpiration <= DateTime.UtcNow)
            {
                return BadRequest(new { Error = "Expired refresh token." });
            }

            var jwtUserDto = new JwtUserDto
            {
                UserName = user.FullName ?? user.UserName!,
                Id = user.Id,
            };

            AuthenticationDto auth = _jwtService.CreateJwtToken(jwtUserDto, true);

            Response.Cookies.Append(
                "refreshToken",
                user.RefreshToken!,
                GetCookieOptions(user.RefreshTokenExpiration)
                );


            AuthenticationResponse authenticationResponse = new AuthenticationResponse
            {
                 Id = user.Id,
                Email = user.Email!,
                UserName = user.UserName!,
                FullName = user.FullName,
                PushNotifications = user.PushNotifications,
                EmailNotifications = user.EmailNotifications,
                AccessToken = auth.AccessToken,
                AccessTokenExpiration = auth.AccessTokenExpiration
            };

            return Ok(authenticationResponse);
        }


        private CookieOptions GetCookieOptions(DateTime? expires)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Path = "/",
                Expires = expires,
                Secure = true,
                SameSite = SameSiteMode.None,
            };
        }
    }
}
