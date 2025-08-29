using System.Net.Http.Json;
using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
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

        private async Task ResetLinesAsync(IEnumerable<Line>? lines = null)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Lines.RemoveRange(db.Lines);

            if (lines != null) await db.Lines.AddRangeAsync(lines);

            await db.SaveChangesAsync();
        }

        #region GetLines
        [Fact]
        public async Task GET_api_lines_ShouldReturn200_AndList()
        {
            // Arrange
            var fakeLines = new List<Line>
            {
                new() { Id = "bakerloo", Code = "BL", Name = "Bakerloo Line", Color = "#894E24" },
                new() { Id = "central", Code = "CL", Name = "Central Line", Color = "#E32017" },
            };

            // Seeds with fake lines
            await ResetLinesAsync(fakeLines);

            //Act
            HttpResponseMessage response = await _client.GetAsync("/api/lines");

            var lines = await response.Content.ReadFromJsonAsync<List<LineDto>>();

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

            var lines = await response.Content.ReadFromJsonAsync<List<LineDto>>();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            lines.Should().NotBeNull();
            lines.Should().BeEmpty();

        }
        #endregion


        #region GetLineByLineId

        [Theory]
        [InlineData("bakerloo")]
        [InlineData("Bakerloo")]
        public async Task GET_api_lines_ShouldReturn200_AndLine(string lineId)
        {
            // Arrange
            var fakeLines = new List<Line>
            {
                new() { Id = "bakerloo", Code = "BL", Name = "Bakerloo Line", Color = "#894E24" },
                new() { Id = "central", Code = "CL", Name = "Central Line", Color = "#E32017" },
            };

            // Seeds with fake lines
            await ResetLinesAsync(fakeLines);

            //Act
            HttpResponseMessage response = await _client.GetAsync($"/api/lines/{lineId}");
            var line = await response.Content.ReadFromJsonAsync<LineDto>();

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            line.Should().NotBeNull();
            line.Id.Should().BeEquivalentTo(fakeLines[0].Id);
        }

        [Fact]
        public async Task GET_api_lines_ShouldReturn404_AndProblemDetail()
        {
            // Arrange
            string invalidId = "InvalidId";
            var fakeLines = new List<Line>
            {
                new() { Id = "bakerloo", Code = "BL", Name = "Bakerloo Line", Color = "#894E24" },
                new() { Id = "central", Code = "CL", Name = "Central Line", Color = "#E32017" },
            };

            // Seeds with fake lines
            await ResetLinesAsync(fakeLines);

            // Act
            HttpResponseMessage response = await _client.GetAsync($"/api/lines/{invalidId}");
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problem.Should().NotBeNull();
            problem.Title.Should().Be("Resource not found");
            problem.Detail.Should().Contain(invalidId);
        }

        #endregion
    }
}