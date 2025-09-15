using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Infrastructure.Data;
using LondonTubeNotifier.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using LondonTubeNotifier.Infrastructure.Entities;

namespace UnitTests.Infrastructure.Repositories
{
    public class UserRepositoryTests
    {
        private ApplicationDbContext GetDbContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        private async Task SeedData(ApplicationDbContext context)
        {
            var line1 = new Line { Id = "victoria", Name = "Victoria" };
            var line2 = new Line { Id = "central", Name = "Central" };

            var user1 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "user1",
                FullName = "User One",
                Email = "user1@test.com",
                Subscriptions = new List<Line> { line1 }
            };

            var user2 = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = "user2",
                FullName = null,
                Email = "user2@test.com",
                Subscriptions = new List<Line> { line1, line2 }
            };

            await context.Users.AddRangeAsync(user1, user2);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetUsersByLineIdAsync_ShouldReturnUsersSubscribedToLine()
        {
            // Arrange
            var context = GetDbContext(nameof(GetUsersByLineIdAsync_ShouldReturnUsersSubscribedToLine));
            await SeedData(context);
            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetUsersByLineIdAsync("victoria", CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
            result.Select(u => u.UserName).Should().Contain(new[] { "user1", "user2" });
        }

        [Fact]
        public async Task GetUsersByLineIdsAsync_ShouldReturnCorrectUserSubscriptionDtos()
        {
            // Arrange
            var context = GetDbContext(nameof(GetUsersByLineIdsAsync_ShouldReturnCorrectUserSubscriptionDtos));
            await SeedData(context);
            var repo = new UserRepository(context);

            var lineIds = new[] { "victoria", "central" };

            // Act
            var result = await repo.GetUsersByLineIdsAsync(lineIds, CancellationToken.None);

            // Assert
            result.Should().HaveCount(3); // user1:1, user2:2
            result.Should().ContainSingle(u => u.FullName == "User One" && u.LineId == "victoria");
            result.Should().ContainSingle(u => u.FullName == "user2" && u.LineId == "central"); 
        }

        [Fact]
        public async Task GetUserWithSubscriptionsAsync_ShouldReturnUser_WhenExists()
        {
            // Arrange
            var context = GetDbContext(nameof(GetUserWithSubscriptionsAsync_ShouldReturnUser_WhenExists));
            await SeedData(context);
            var repo = new UserRepository(context);

            var existingUser = await context.Users.FirstAsync();

            // Act
            var result = await repo.GetUserWithSubscriptionsAsync(existingUser.Id, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(existingUser.Id);
            result.Subscriptions.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetUserWithSubscriptionsAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var context = GetDbContext(nameof(GetUserWithSubscriptionsAsync_ShouldReturnNull_WhenUserDoesNotExist));
            await SeedData(context);
            var repo = new UserRepository(context);

            // Act
            var result = await repo.GetUserWithSubscriptionsAsync(Guid.NewGuid(), CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
