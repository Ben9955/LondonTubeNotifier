using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using IntegrationTests.Factory;
using IntegrationTests.Helpers;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

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

        private void ResetCache()
        {
            using var scope = _factory.Services.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
            cache.Remove("LinesCache");
        }

        private async Task ResetAllAsync()
        {
            await ResetLinesAsync();
            await ResetUsersAsync();
            ResetCache(); 
        }

        private void Authenticate() =>
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme", "11111111-1111-1111-1111-111111111111");

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
            //await ResetLinesAsync();
            //await ResetUsersAsync();

            await ResetAllAsync();
            Authenticate();

            // First subscription should succeed
            var firstResponse = await _httpClient.PostAsync("/api/subscriptions/line/central", null);
            var content1 = await firstResponse.Content.ReadAsStringAsync();
            firstResponse.StatusCode.Should().Be(HttpStatusCode.NoContent,
                because: $"Expected 400 BadRequest, but got {firstResponse.StatusCode}. Response content: {content1}"
                );

            // Second subscription should fail (already subscribed)
            var secondResponse = await _httpClient.PostAsync("/api/subscriptions/line/central", null);

            // Read content
            var content = await secondResponse.Content.ReadAsStringAsync();

            // Assert and include the response content in the failure message
            secondResponse.StatusCode.Should().Be(
                HttpStatusCode.BadRequest,
                because: $"Expected 400 BadRequest, but got {secondResponse.StatusCode}. Response content: {content}");
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
