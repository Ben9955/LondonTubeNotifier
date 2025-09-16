using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace UnitTests.Infrastructure.Repositories
{
    public class CachedLineStatusRepositoryTests
    {
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILineStatusRepository> _innerRepositoryMock;
        private readonly CachedLineStatusRepository _repository;

        public CachedLineStatusRepositoryTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _innerRepositoryMock = new Mock<ILineStatusRepository>();
            _repository = new CachedLineStatusRepository(_memoryCache, _innerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetLatestLineStatusesAsync_ShouldReturnFromInnerRepository_WhenCacheEmpty()
        {
            // Arrange
            var data = new Dictionary<string, HashSet<LineStatus>>
            {
                { "victoria", new HashSet<LineStatus> { new LineStatus { LineId = "victoria" } } }
            };

            _innerRepositoryMock
                .Setup(r => r.GetLatestLineStatusesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(data);

            // Act
            var result = await _repository.GetLatestLineStatusesAsync(CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(data);

            // Confirm it is cached
            _memoryCache.TryGetValue("LineStatusesCache", out Dictionary<string, HashSet<LineStatus>> cached);
            cached.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async Task GetLastStatusForLineAsync_ShouldReturnStatuses_WhenLineExistsInCache()
        {
            // Arrange
            var data = new Dictionary<string, HashSet<LineStatus>>
            {
                { "victoria", new HashSet<LineStatus> { new LineStatus { LineId = "victoria" } } }
            };
            _memoryCache.Set("LineStatusesCache", data);

            // Act
            var result = await _repository.GetLastStatusForLineAsync("victoria", CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(data["victoria"]);
        }

        [Fact]
        public async Task GetLastStatusForLineAsync_ShouldReturnEmpty_WhenLineDoesNotExist()
        {
            // Arrange
            var data = new Dictionary<string, HashSet<LineStatus>>();
            _memoryCache.Set("LineStatusesCache", data);

            // Act
            var result = await _repository.GetLastStatusForLineAsync("nonexistent", CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SaveStatusAsync_ShouldSaveToInnerRepositoryAndCache()
        {
            // Arrange
            var statuses = new Dictionary<string, HashSet<LineStatus>>
            {
                { "victoria", new HashSet<LineStatus> { new LineStatus { LineId = "victoria" } } }
            };

            // Act
            await _repository.SaveStatusAsync(statuses, CancellationToken.None);

            // Assert
            _innerRepositoryMock.Verify(r => r.SaveStatusAsync(statuses, It.IsAny<CancellationToken>()), Times.Once);

            _memoryCache.TryGetValue("LineStatusesCache", out Dictionary<string, HashSet<LineStatus>> cached);
            cached.Should().BeEquivalentTo(statuses);
        }
    }
}

