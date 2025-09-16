using FluentAssertions;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.WebApi.Controllers;
using LondonTubeNotifier.WebApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.WebApi
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManager;
        private readonly Mock<IJwtService> _jwtService;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            _jwtService = new Mock<IJwtService>();
            _controller = new AccountController(_userManager.Object, _jwtService.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUsernameExists()
        {
            var request = new RegisterRequest { UserName = "existing", Email = "email@test.com" };
            _userManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync(new ApplicationUser());

            var result = await _controller.Register(request);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenSuccessful()
        {
            var request = new RegisterRequest { UserName = "newuser", Email = "new@test.com", Password = "Pass123!" };
            _userManager.Setup(u => u.FindByNameAsync(request.UserName)).ReturnsAsync((ApplicationUser)null);
            _userManager.Setup(u => u.FindByEmailAsync(request.Email)).ReturnsAsync((ApplicationUser)null);
            _userManager.Setup(u => u.CreateAsync(It.IsAny<ApplicationUser>(), request.Password)).ReturnsAsync(IdentityResult.Success);
            _jwtService.Setup(j => j.CreateJwtToken(It.IsAny<JwtUserDto>())).Returns(new AuthenticationDto
            {
                AccessToken = "token",
                RefreshToken = "refresh",
                AccessTokenExpiration = DateTime.UtcNow.AddHours(1),
                RefreshTokenExpiration = DateTime.UtcNow.AddHours(5)
            });

            var result = await _controller.Register(request);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenInvalidCredentials()
        {
            var request = new LoginRequest { EmailOrUsername = "user@test.com", Password = "wrongpass" };
            _userManager.Setup(u => u.FindByNameAsync(request.EmailOrUsername)).ReturnsAsync((ApplicationUser)null);
            _userManager.Setup(u => u.FindByEmailAsync(request.EmailOrUsername)).ReturnsAsync((ApplicationUser)null);

            var result = await _controller.Login(request);

            result.Result.Should().BeOfType<UnauthorizedObjectResult>();
        }
    }
}

