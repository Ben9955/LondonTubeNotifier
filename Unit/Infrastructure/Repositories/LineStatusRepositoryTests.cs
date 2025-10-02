using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace UnitTests.Infrastructure.Repositories
{
    public class LineStatusRepositoryTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly LineStatusRepository _repository;

        public LineStatusRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)) 
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _repository = new LineStatusRepository(_dbContext);
        }

        [Fact]
        public async Task SaveStatusAsync_ShouldSaveStatusesAndRemoveOld()
        {
            // Arrange
            _dbContext.LineStatuses.Add(new LineStatus { LineId = "line1", StatusDescription = "Old" });
            await _dbContext.SaveChangesAsync();

            var newStatuses = new Dictionary<string, List<LineStatus>>
            {
                ["line1"] = new List<LineStatus> { new LineStatus { LineId = "line1", StatusDescription = "New" } },
                ["line2"] = new List<LineStatus> { new LineStatus { LineId = "line2", StatusDescription = "New2" } }
            };

            // Act
            await _repository.UpdateLinesAsync(newStatuses, CancellationToken.None);

            // Assert
            var allStatuses = await _dbContext.LineStatuses.ToListAsync();
            allStatuses.Should().HaveCount(2);
            allStatuses.Should().ContainSingle(s => s.LineId == "line1" && s.StatusDescription == "New");
            allStatuses.Should().ContainSingle(s => s.LineId == "line2" && s.StatusDescription == "New2");
        }

        [Fact]
        public async Task SaveStatusAsync_WithEmptyDictionary_ShouldClearDatabase()
        {
            // Arrange
            _dbContext.LineStatuses.Add(new LineStatus { LineId = "line1", StatusDescription = "Old" });
            await _dbContext.SaveChangesAsync();

            var emptyStatuses = new Dictionary<string, List<LineStatus>>();

            // Act
            await _repository.UpdateLinesAsync(emptyStatuses, CancellationToken.None);

            // Assert
            var allStatuses = await _dbContext.LineStatuses.ToListAsync();
            allStatuses.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLastStatusForLineAsync_ShouldReturnCorrectStatuses()
        {
            // Arrange
            var statuses = new[]
            {
                new LineStatus { LineId = "line1", StatusDescription = "Status1" },
                new LineStatus { LineId = "line1", StatusDescription = "Status2" }
            };
            _dbContext.LineStatuses.AddRange(statuses);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetLastStatusForLineAsync("line1", CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(s => s.StatusDescription).Should().Contain(new[] { "Status1", "Status2" });
        }

        [Fact]
        public async Task GetLastStatusForLineAsync_NoStatuses_ShouldReturnEmpty()
        {
            // Act
            var result = await _repository.GetLastStatusForLineAsync("lineX", CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLatestLineStatusesAsync_ShouldReturnGroupedStatuses()
        {
            // Arrange
            var statuses = new[]
            {
                new LineStatus { LineId = "line1", StatusDescription = "Status1" },
                new LineStatus { LineId = "line2", StatusDescription = "Status2" }
            };
            _dbContext.LineStatuses.AddRange(statuses);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetLatestLineStatusesAsync(CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result["line1"].Should().ContainSingle(s => s.StatusDescription == "Status1");
            result["line2"].Should().ContainSingle(s => s.StatusDescription == "Status2");
        }

        [Fact]
        public async Task GetLatestLineStatusesAsync_NoStatuses_ShouldReturnEmptyDictionary()
        {
            // Act
            var result = await _repository.GetLatestLineStatusesAsync(CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveStatusAsync_WhenExceptionOccurs_ShouldRollback()
        {
            // Arrange
            // Forcing an exception by adding a null LineId
            var badStatuses = new Dictionary<string, List<LineStatus>>
            {
                ["line1"] = new List<LineStatus>
                {
                    new LineStatus { LineId = null!, StatusDescription = "Bad" }
                }
            };

            // Act
            Func<Task> act = async () => await _repository.UpdateLinesAsync(badStatuses, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DbUpdateException>();
            (await _dbContext.LineStatuses.ToListAsync()).Should().BeEmpty(); // rollback ensured
        }
    }
}

