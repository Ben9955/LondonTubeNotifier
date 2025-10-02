using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Factory;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using LondonTubeNotifier.WebApi.Dtos;
using IntegrationTests.Helpers;

namespace IntegrationTests.Integration
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

        private async Task ResetUsersAsync(bool emptyTable = false)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await Utilities.ResetUsersAsync(db, userManager, emptyTable);
        }


        #region Register

        [Fact]
        public async Task Register_ShouldReturn200_WhenValid()
        {
            await ResetUsersAsync(true);

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

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.UserName.Should().Be(request.UserName);
            result.Email.Should().Be(request.Email);
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
            //result.RefreshToken.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public async Task Register_ShouldReturn400_WhenUsernameTaken()
        {
            await ResetUsersAsync();

            var request = new RegisterRequest
            {
                UserName = "existinguser1",
                Email = "existing3@example.com",
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/register", request);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should().ContainKey("error");
            result["error"].Should().Be("Username already taken");
        }

        [Fact]
        public async Task Register_ShouldReturn400_WhenEmailTaken()
        {
            await ResetUsersAsync();

            var request = new RegisterRequest
            {
                UserName = "newuser",
                Email = "existing1@example.com",
                FullName = "New User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/register", request);
            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.Should().ContainKey("error");
            result["error"].Should().Be("Email already registered");
        }

        #endregion

        #region Login

        [Fact]
        public async Task Login_ShouldReturn200_WhenValid()
        {
            await ResetUsersAsync();
            var request = new LoginRequest
            {
                EmailOrUsername = "existinguser1",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            var result = await response.Content.ReadFromJsonAsync<AuthenticationResponse>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Should().NotBeNull();
            result.UserName.Should().Be("existinguser1");
            result.AccessToken.Should().NotBeNullOrWhiteSpace();
        }


        [Fact]
        public async Task Login_ShouldReturn401_WhenInvalidPassword()
        {
            await ResetUsersAsync();

            var request = new LoginRequest
            {
                EmailOrUsername = "loginuser2",
                Password = "WrongPassword!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ShouldReturn401_WhenUserNotFound()
        {
            await ResetUsersAsync(true);

            var request = new LoginRequest
            {
                EmailOrUsername = "unknownuser",
                Password = "Password123!"
            };

            var response = await _client.PostAsJsonAsync("/api/account/login", request);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region Logout

        [Fact]
        public async Task Logout_ShouldReturn204()
        {
            await ResetUsersAsync();

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer faketoken");
            var response = await _client.GetAsync("/api/account/logout");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        #endregion
    }
}


