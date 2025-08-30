using System.Net.Http.Json;
using FluentAssertions;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.WebApi.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public class AccountControllerIntegrationTest : IClassFixture<WebFactory>
    {
        private readonly HttpClient _client;
        private readonly WebFactory _factory;

        public AccountControllerIntegrationTest(WebFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task ResetUsersAsync(IEnumerable<ApplicationUser>? users = null)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Clear all users
            db.Users.RemoveRange(db.Users);
            await db.SaveChangesAsync();

            // Recreate users via Identity
            if (users != null)
            {
                foreach (var user in users)
                {
                    var result = await userManager.CreateAsync(user, "Password123!");
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to seed user {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }


        #region Register

        [Fact]
        public async Task POST_api_account_register_ShouldReturn200_WhenValid()
        {
            await ResetUsersAsync();

            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                FullName = "Test User",
                PhoneNumber = "1234567890",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/register", request);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.UserName.Should().Be(request.UserName);
            result.Email.Should().Be(request.Email);
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task POST_api_account_register_ShouldReturn400_WhenUsernameTaken()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "existinguser",
                Email = "existing@example.com",
                FullName = "Existing User"
            };
            await ResetUsersAsync(new[] { existingUser });

            var request = new RegisterRequest
            {
                UserName = "existinguser",
                Email = "newemail@example.com",
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/register", request);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            result.Should().ContainKey("error");
            result["error"].Should().Be("Username already taken");
        }

        [Fact]
        public async Task POST_api_account_register_ShouldReturn400_WhenEmailTaken()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "existinguser",
                Email = "taken@example.com",
                FullName = "Existing User"
            };
            await ResetUsersAsync(new[] { existingUser });

            var request = new RegisterRequest
            {
                UserName = "newuser",
                Email = "taken@example.com",
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/register", request);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            Console.WriteLine(result);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            result.Should().ContainKey("error");
            result["error"].Should().Be("Email already registered");
        }

        #endregion

        #region Login

        [Fact]
        public async Task POST_api_account_login_ShouldReturn200_WhenValid()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "loginuser",
                Email = "login@example.com",
                FullName = "Login User"
            };

            await ResetUsersAsync(new[] { existingUser });

            var request = new LoginRequest
            {
                EmailOrUsername = "loginuser",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.UserName.Should().Be(existingUser.UserName);
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task POST_api_account_login_ShouldReturn401_WhenInvalidPassword()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "loginuser2",
                Email = "login2@example.com",
                FullName = "Login User 2"
            };
            await ResetUsersAsync(new[] { existingUser });

            var request = new LoginRequest
            {
                EmailOrUsername = "loginuser2",
                Password = "WrongPassword!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task POST_api_account_login_ShouldReturn401_WhenUserNotFound()
        {
            await ResetUsersAsync();

            var request = new LoginRequest
            {
                EmailOrUsername = "unknownuser",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Logout

        [Fact]
        public async Task GET_api_account_logout_ShouldReturn204()
        {
            var existingUser = new ApplicationUser
            {
                UserName = "logoutuser",
                Email = "logout@example.com",
                FullName = "Logout User"
            };
            await ResetUsersAsync(new[] { existingUser });

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer faketoken");

            var response = await _client.GetAsync("/api/account/logout");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        #endregion
    }
}


