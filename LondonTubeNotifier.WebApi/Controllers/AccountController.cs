using System.Security.Claims;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.WebApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LondonTubeNotifier.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtService _jwtService;
        public AccountController(UserManager<ApplicationUser> userManager, IJwtService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

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

           IdentityResult result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                // Return validation errors from Identity
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            JwtUserDto jwtUserDto = new JwtUserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            AuthenticationDto authentication = _jwtService.CreateJwtToken(jwtUserDto);

            // save refreshToken to the database
            user.RefreshToken = authentication.RefreshToken;
            user.RefreshTokenExpiration = authentication.RefreshTokenExpiration;

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new EntityUpdateException("An error occurred while updating the user.", ex);
            }

            AuthenticationResponse response = new AuthenticationResponse()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                RefreshToken = authentication.RefreshToken,
                AccessToken = authentication.AccessToken,
                AccessTokenExpiration = authentication.AccessTokenExpiration,
            };

            return Ok(response);
        }


        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.EmailOrUsername)
            ?? await _userManager.FindByEmailAsync(request.EmailOrUsername);

            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            JwtUserDto jwtUserDto = new JwtUserDto { Id = user.Id, UserName = user.UserName! };

            AuthenticationDto authentication = _jwtService.CreateJwtToken(jwtUserDto);

            user.RefreshToken = authentication.RefreshToken;
            user.RefreshTokenExpiration = authentication.RefreshTokenExpiration;

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                throw new EntityUpdateException("An error occurred while updating the user.", ex);
            }

            AuthenticationResponse response = new AuthenticationResponse()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                RefreshToken = authentication.RefreshToken,
                AccessToken = authentication.AccessToken,
                AccessTokenExpiration = authentication.AccessTokenExpiration
            };

            return Ok(response);
        }


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

                    try
                    {
                        await _userManager.UpdateAsync(user);
                    }
                    catch (Exception ex)
                    {
                        throw new EntityUpdateException("Failed to clear refresh token.", ex);
                    }
                }
            }

            return NoContent();
        }
    }
}
