using System.Security.Claims;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.WebApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AuthenticationResponse>> Register(RegisterRequest request)
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
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            var jwtUser = new JwtUserDto { Id = user.Id, UserName = user.UserName };
            var auth = _jwtService.CreateJwtToken(jwtUser);

            user.RefreshToken = auth.RefreshToken;
            user.RefreshTokenExpiration = auth.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                RefreshToken = auth.RefreshToken,
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
                return Unauthorized("Invalid credentials");

            var jwtUser = new JwtUserDto { Id = user.Id, UserName = user.UserName! };
            var auth = _jwtService.CreateJwtToken(jwtUser);

            user.RefreshToken = auth.RefreshToken;
            user.RefreshTokenExpiration = auth.RefreshTokenExpiration;
            await _userManager.UpdateAsync(user);

            var response = new AuthenticationResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                RefreshToken = auth.RefreshToken,
                AccessToken = auth.AccessToken,
                AccessTokenExpiration = auth.AccessTokenExpiration
            };

            return Ok(response);
        }

        /// <summary>
        /// Logs out the user by clearing their refresh token.
        /// </summary>
        /// <returns>No content on success.</returns>
        /// <response code="204">User logged out successfully.</response>
        /// <remarks>Clears refresh token from the database.</remarks>
        [HttpGet("logout")]
        public async Task<IActionResult> GetLogout()
        {
            var userId = User.FindFirstValue("sub");
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.RefreshToken = null;
                    user.RefreshTokenExpiration = null;
                    await _userManager.UpdateAsync(user);
                }
            }
            return NoContent();
        }
    }
}
