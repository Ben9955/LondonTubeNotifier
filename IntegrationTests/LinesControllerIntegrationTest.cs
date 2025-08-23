using System.Net.Http.Json;
using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
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

        [Fact]
        public async Task GET_api_lines_ShouldReturn200_AndList()
        {
            // Arrange
            var fakeLines = new List<Line>
            {
                new() { Id = "Bakerloo", Code = "BL", Name = "Bakerloo Line", Color = "#894E24" },
                new() { Id = "Central", Code = "CL", Name = "Central Line", Color = "#E32017" },
            };

            // Seeds with fake lines
            await ResetLinesAsync(fakeLines);

            //Act
            HttpResponseMessage response = await _client.GetAsync("/api/lines");

            var lines = await response.Content.ReadFromJsonAsync<List<LineResponseDTO>>();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            lines.Should().NotBeNull();
            lines.Should().HaveCount(fakeLines.Count);
            lines.Select(l => new { l.Id, l.Code, l.Name, l.Color })
                .Should().BeEquivalentTo(fakeLines.Select(f => new { f.Id, f.Code, f.Name, f.Color }));

        }

        [Fact]
        public async Task GET_api_lines_ShouldReturn200_AndEmptyList()
        {
            await ResetLinesAsync();

            //Act
            HttpResponseMessage response = await _client.GetAsync("/api/lines");

            var lines = await response.Content.ReadFromJsonAsync<List<LineResponseDTO>>();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            lines.Should().NotBeNull();
            lines.Should().BeEmpty();

        }


        private async Task ResetLinesAsync(IEnumerable<Line>? lines = null)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Lines.RemoveRange(db.Lines);

            if(lines != null) await db.Lines.AddRangeAsync(lines);

            await db.SaveChangesAsync();
        }

    }
}