using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.Interfaces;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace UnitTests.Infrastructure.Repositories
{
    public class CachedLineRepositoryTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILineRepository> _innerRepositoryMock;
        private readonly CachedLineRepository _repository;

        public CachedLineRepositoryTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _innerRepositoryMock = new Mock<ILineRepository>();
            _repository = new CachedLineRepository(_memoryCache, _innerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetLinesAsync_ShouldReturnFromInnerRepositoryAndCacheIt()
        {
            // Arrange
            var lines = new List<Line>
            {
                new Line { Id = "victoria" },
                new Line { Id = "central" }
            };

            _innerRepositoryMock.Setup(r => r.GetLinesAsync()).ReturnsAsync(lines);

            // Act
            var result = await _repository.GetLinesAsync();

            // Assert
            result.Should().BeEquivalentTo(lines);

            // Ensure cache is populated
            _memoryCache.TryGetValue("LinesCache", out List<Line> cachedLines);
            cachedLines.Should().BeEquivalentTo(lines);
        }

        [Fact]
        public async Task GetLinesAsync_ShouldReturnFromCache_WhenAlreadyCached()
        {
            // Arrange
            var cachedLines = new List<Line>
            {
                new Line { Id = "bakerloo" }
            };
            _memoryCache.Set("LinesCache", cachedLines);

            // Act
            var result = await _repository.GetLinesAsync();

            // Assert
            result.Should().BeEquivalentTo(cachedLines);
            _innerRepositoryMock.Verify(r => r.GetLinesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetLineByLineIdAsync_ShouldReturnLine_WhenExists()
        {
            // Arrange
            var lines = new List<Line>
            {
                new Line { Id = "victoria" },
                new Line { Id = "central" }
            };
            _memoryCache.Set("LinesCache", lines);

            // Act
            var result = await _repository.GetLineByLineIdAsync("central");

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be("central");
        }

        [Fact]
        public async Task GetLineByLineIdAsync_ShouldReturnNull_WhenLineDoesNotExist()
        {
            // Arrange
            var lines = new List<Line>
            {
                new Line { Id = "victoria" }
            };
            _memoryCache.Set("LinesCache", lines);

            // Act
            var result = await _repository.GetLineByLineIdAsync("central");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddSubscriptionAsync_ShouldCallInnerRepository()
        {
            // Arrange
            var userMock = new Mock<IUser>();
            var line = new Line { Id = "victoria" };

            // Act
            await _repository.AddSubscriptionAsync(userMock.Object, line);

            // Assert
            _innerRepositoryMock.Verify(r => r.AddSubscriptionAsync(userMock.Object, line), Times.Once);
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldCallInnerRepository()
        {
            // Arrange
            var userMock = new Mock<IUser>();
            var line = new Line { Id = "victoria" };

            // Act
            await _repository.DeleteSubscriptionAsync(userMock.Object, line);

            // Assert
            _innerRepositoryMock.Verify(r => r.DeleteSubscriptionAsync(userMock.Object, line), Times.Once);
        }
    }
}
