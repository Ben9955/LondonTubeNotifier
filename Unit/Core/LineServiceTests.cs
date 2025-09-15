using AutoFixture;
using Microsoft.Extensions.Logging;
using LondonTubeNotifier.Core.Domain.RespositoryContracts;
using LondonTubeNotifier.Core.MapperContracts;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Core.Services;
using Moq;
using FluentAssertions;
using LondonTubeNotifier.Core.Domain.Entities;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.Exceptions;

namespace UnitTests.Core
{
    public class LineServiceTests
    {
        private readonly Mock<ILineRepository> _lineRepository;
        private readonly Mock<ILineMapper> _lineMapper;
        private readonly Mock<ILogger<LineService>> _logger;
        private readonly Fixture _fixture;
        private readonly ILineService _lineService;

        public LineServiceTests()
        {
            _fixture = new Fixture();
            _fixture.Behaviors
                .OfType<ThrowingRecursionBehavior>()
                .ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _lineMapper = new Mock<ILineMapper>();
            _lineRepository = new Mock<ILineRepository>();
            _logger = new Mock<ILogger<LineService>>();

            _lineService = new LineService(
                _lineRepository.Object,
                _lineMapper.Object,
                _logger.Object);
        }



        #region GetLines

        [Fact]
        public async Task GetLinesAsync_ShouldReturnListOfDtos_RepositoryReturnsNoLines()
        {
            //Arrange
            var lines = _fixture.CreateMany<Line>(4).ToList();
            var linesDtos = _fixture.CreateMany<LineDto>(4).ToList();

            _lineRepository.Setup(r => r.GetLinesAsync()).ReturnsAsync(lines);
            _lineMapper.Setup(m => m.ToDtoList(lines)).Returns(linesDtos);

            // Act
            var result = await _lineService.GetLinesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(4);
            result.Should().BeEquivalentTo(linesDtos);
            _lineMapper.Verify(m => m.ToDtoList(lines), Times.Once);
        }




        [Fact]
        public async Task GetLinesAsync_ShouldReturnEmptyList_WhenNoLineExist()
        {
            //Arrange
            var lines = new List<Line>();
            var linesDtos = new List<LineDto>();

            _lineRepository.Setup(r => r.GetLinesAsync()).ReturnsAsync(lines);
            _lineMapper.Setup(m => m.ToDtoList(lines)).Returns(linesDtos);

            // Act
            var result = await _lineService.GetLinesAsync();

            //Assert
            result.Should().BeEmpty();
            _lineRepository.Verify(r => r.GetLinesAsync(), Times.Once);
            _lineMapper.Verify(m => m.ToDtoList(lines), Times.Once);
        }

        #endregion


        #region GetLineByLineId

        [Fact]
        public async Task GetLineByLineIdAsync_ShouldReturnLineDto_WhenValidLineId()
        {
            //Arrange
            var line = _fixture.Create<Line>();
            var lineDto = _fixture.Create<LineDto>();

            _lineRepository.Setup(r => r.GetLineByLineIdAsync(It.IsAny<string>())).ReturnsAsync(line);
            _lineMapper.Setup(m => m.ToDto(It.Is<Line>(l => l.Id == line.Id))).Returns(lineDto);

            // Act
            var result = await _lineService.GetLineByLineIdAsync(line.Id);

            //Assert
            result.Should().Be(lineDto);
            _lineMapper.Verify(m => m.ToDto(It.Is<Line>(l => l.Id == line.Id)), Times.Once);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetLineByLineIdAsync_ShouldThrowDomainValidationException_WhenLineIdIsInvalid(string lineId)
        {

            // Act
            Func<Task> action = async () => await _lineService.GetLineByLineIdAsync(lineId!);

            //Assert
            await action.Should().ThrowAsync<DomainValidationException>();

            _lineRepository.Verify(r => r.GetLineByLineIdAsync(It.IsAny<string>()), Times.Never);
            _lineMapper.Verify(m => m.ToDto(It.IsAny<Line>()), Times.Never);
        }


        [Fact]
        public async Task GetLineByLineIdAsync_ShouldThrowDomainNotFoundException_WhenLineDoesNotExist()
        {

            //Arrange
            string lineId = "InvalidId";
            _lineRepository.Setup(r => r.GetLineByLineIdAsync(lineId)).ReturnsAsync((Line?)null);

            // Act
            Func<Task> action = async () => await _lineService.GetLineByLineIdAsync(lineId);

            //Assert
            await action.Should().ThrowAsync<DomainNotFoundException>();
            _lineMapper.Verify(m => m.ToDto(It.IsAny<Line>()), Times.Never);
        }

        #endregion

    }
}