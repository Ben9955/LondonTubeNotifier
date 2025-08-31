using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using IntegrationTests.Factory;
using IntegrationTests.Helpers;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace IntegrationTests.Integration
{
    [Collection("Database")]
    public class SubscriptionsControllerIntegrationTest : IClassFixture<WebFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly WebFactory _factory;

        public SubscriptionsControllerIntegrationTest(WebFactory factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private async Task ResetLinesAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await Utilities.ResetLinesAsync(db);
        }

        private async Task ResetUsersAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await Utilities.ResetUsersAsync(db, userManager);
        }

        private void Authenticate() =>
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme", "anything");

        #region SubscribeToLine

        [Fact]
        public async Task SubscribeToLine_ShouldReturn204_WhenAuthenticated()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            Authenticate();

            var response = await _httpClient.PostAsync("/api/subscriptions/line/central", null);
            var content = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(HttpStatusCode.NoContent, because: $"Response content: {content}");
        }

        [Fact]
        public async Task SubscribeToLine_ShouldReturn400_WhenAlreadySubscribed()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            Authenticate();

            (await _httpClient.PostAsync("/api/subscriptions/line/central", null))
                .StatusCode.Should().Be(HttpStatusCode.NoContent);

            (await _httpClient.PostAsync("/api/subscriptions/line/central", null))
                .StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SubscribeToLine_ShouldReturn404_WhenLineDoesNotExist()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            Authenticate();

            var response = await _httpClient.PostAsync("/api/subscriptions/line/invalidline", null);
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task SubscribeToLine_ShouldReturn401_WhenNotAuthenticated()
        {
            await ResetLinesAsync();
            var response = await _httpClient.PostAsync("/api/subscriptions/line/central", null);
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region UnsubscribeToLine

        [Fact]
        public async Task UnsubscribeToLine_ShouldReturn204_WhenAuthenticated()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            Authenticate();

            (await _httpClient.PostAsync("/api/subscriptions/line/central", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);
            (await _httpClient.DeleteAsync("/api/subscriptions/line/central")).StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UnsubscribeToLine_ShouldReturn401_WhenNotAuthenticated()
        {

            var response = await _httpClient.DeleteAsync("/api/subscriptions/line/central");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion

        #region GetLinesForUser

        [Fact]
        public async Task GetLinesForUser_ShouldReturn200_WhenAuthenticated()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            Authenticate();

            var response = await _httpClient.GetAsync("/api/subscriptions");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetLinesForUser_ShouldReturn401_WhenNotAuthenticated()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            var response = await _httpClient.GetAsync("/api/subscriptions");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        #endregion
    }
}
