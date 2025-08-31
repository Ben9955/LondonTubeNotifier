using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using IntegrationTests.Factory;
using IntegrationTests.Helpers;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Integration
{
    public class LinesControllerIntegrationTest : IClassFixture<WebFactory>
    {
        private readonly HttpClient _client;
        private readonly WebFactory _factory;

        public LinesControllerIntegrationTest(WebFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        private async Task ResetLinesAsync(bool emptyTable = false)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await Utilities.ResetLinesAsync(db, emptyTable);
        }

        #region GetLines

        [Fact]
        public async Task GetLines_ShouldReturn200_AndList()
        {
            var fakeLines = Utilities.GetSeedingLines();
            await ResetLinesAsync();

            var response = await _client.GetAsync("/api/lines");
            var lines = await response.Content.ReadFromJsonAsync<List<LineDto>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            lines.Should().NotBeNull();
            lines.Should().HaveCount(fakeLines.Count);
            lines.Select(l => new { l.Id, l.Code, l.Name, l.Color })
                 .Should().BeEquivalentTo(fakeLines.Select(f => new { f.Id, f.Code, f.Name, f.Color }));
        }

        [Fact]
        public async Task GetLines_ShouldReturn200_AndEmptyList()
        {
            await ResetLinesAsync(true);

            var response = await _client.GetAsync("/api/lines");
            var lines = await response.Content.ReadFromJsonAsync<List<LineDto>>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            lines.Should().NotBeNull();
            lines.Should().BeEmpty();
        }

        #endregion

        #region GetLineById

        [Theory]
        [InlineData("bakerloo")]
        [InlineData("Bakerloo")]
        public async Task GetLineById_ShouldReturn200_AndLine(string lineId)
        {
            var fakeLines = Utilities.GetSeedingLines();
            await ResetLinesAsync();

            var response = await _client.GetAsync($"/api/lines/{lineId}");
            var line = await response.Content.ReadFromJsonAsync<LineDto>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            line.Should().NotBeNull();
            line.Id.Should().BeEquivalentTo(fakeLines[0].Id);
        }

        [Fact]
        public async Task GetLineById_ShouldReturn404_AndProblemDetail()
        {
            string invalidId = "InvalidId";
            await ResetLinesAsync();

            var response = await _client.GetAsync($"/api/lines/{invalidId}");
            var problem = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            problem.Should().NotBeNull();
            problem.Title.Should().Be("Resource not found");
            problem.Detail.Should().Contain(invalidId);
        }

        #endregion
    }
}
