using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Entities;
using LondonTubeNotifier.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace UnitTests.Infrastructure.Repositories
{
    public class LineRepositoryTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new ApplicationDbContext(options);
        }

        private IUser CreateTestUser()
        {
            return new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Subscriptions = new List<Line>()
            };
        }

        private Line CreateTestLine(string id = "victoria")
        {
            return new Line
            {
                Id = id,
                Name = "Victoria Line"
            };
        }

        [Fact]
        public async Task GetLinesAsync_ShouldReturnAllLines()
        {
            // Arrange
            var dbContext = CreateDbContext();
            dbContext.Lines.AddRange(CreateTestLine("victoria"), CreateTestLine("bakerloo"));
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            var result = await repo.GetLinesAsync();

            // Assert
            result.Should().HaveCount(2);
            result.Select(l => l.Id).Should().Contain(new[] { "victoria", "bakerloo" });
        }

        [Fact]
        public async Task GetLineByLineIdAsync_ShouldReturnLine_WhenExists()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var line = CreateTestLine("central");
            dbContext.Lines.Add(line);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            var result = await repo.GetLineByLineIdAsync("central");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("central");
        }

        [Fact]
        public async Task GetLineByLineIdAsync_ShouldReturnNull_WhenDoesNotExist()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var repo = new LineRepository(dbContext);

            // Act
            var result = await repo.GetLineByLineIdAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddSubscriptionAsync_ShouldAddLine_WhenNotSubscribed()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var user = CreateTestUser();
            var line = CreateTestLine("victoria");
            dbContext.Users.Add((ApplicationUser)user);
            dbContext.Lines.Add(line);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            await repo.AddSubscriptionAsync(user, line);

            // Assert
            user.Subscriptions.Should().ContainSingle(l => l.Id == "victoria");
        }

        [Fact]
        public async Task AddSubscriptionAsync_ShouldNotDuplicateSubscription_WhenAlreadySubscribed()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var user = CreateTestUser();
            var line = CreateTestLine("victoria");
            user.Subscriptions.Add(line);
            dbContext.Users.Add((ApplicationUser)user);
            dbContext.Lines.Add(line);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            await repo.AddSubscriptionAsync(user, line);

            // Assert
            user.Subscriptions.Should().HaveCount(1);
        }

        [Fact]
        public async Task AddSubscriptionAsync_ShouldAttachLine_WhenDetached()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var user = CreateTestUser();
            var line = CreateTestLine("victoria");
            dbContext.Users.Add((ApplicationUser)user);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            await repo.AddSubscriptionAsync(user, line);

            // Assert
            dbContext.Entry(line).State.Should().Be(EntityState.Unchanged);
            user.Subscriptions.Should().ContainSingle(l => l.Id == "victoria");
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldRemoveLine_WhenSubscribed()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var user = CreateTestUser();
            var line = CreateTestLine("victoria");
            user.Subscriptions.Add(line);
            dbContext.Users.Add((ApplicationUser)user);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            await repo.DeleteSubscriptionAsync(user, line);

            // Assert
            user.Subscriptions.Should().BeEmpty();
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldDoNothing_WhenNotSubscribed()
        {
            // Arrange
            var dbContext = CreateDbContext();
            var user = CreateTestUser();
            var line = CreateTestLine("victoria");
            dbContext.Users.Add((ApplicationUser)user);
            await dbContext.SaveChangesAsync();

            var repo = new LineRepository(dbContext);

            // Act
            await repo.DeleteSubscriptionAsync(user, line);

            // Assert
            user.Subscriptions.Should().BeEmpty();
        }
    }
}
