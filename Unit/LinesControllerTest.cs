using AutoFixture;
using FluentAssertions;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.WebApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    public class LinesControllerTest
    {
        private readonly Mock<ILineService> _lineService;
        private readonly Fixture _fixture;
        private readonly LinesController _linesController;
        private readonly Mock<ILogger<LinesController>> _logger;

        public LinesControllerTest()
        {
            _lineService = new Mock<ILineService>();
            _fixture = new Fixture();
            _logger = new Mock<ILogger<LinesController>>();
            _linesController = new LinesController(_lineService.Object, _logger.Object);
        }


        [Fact]
        public async Task GetLines_ShouldReturnOkWithList_WhenServiceReturnsLines()
        { 
            //Arrange
            var lines = _fixture.CreateMany<LineResponseDTO>(3).ToList();
            _lineService.Setup(l => l.GetLinesAsync()).ReturnsAsync(lines);

            //Act
            var result = await _linesController.GetLines();

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<List<LineResponseDTO>>()
                .Which.Should().BeEquivalentTo(lines);
            _lineService.Verify(s => s.GetLinesAsync(), Times.Once);
        }


        [Fact]
        public async Task GetLine_ShouldReturnOkWithLine_WhenServiceReturnsLine()
        {
            //Arrange
            var line = _fixture.Create<LineResponseDTO>();
            _lineService.Setup(l => l.GetLineByLineIdAsync(line.Id)).ReturnsAsync(line);

            //Act
            var result = await _linesController.GetLine(line.Id);

            //Assert
            result.Result.Should().BeOfType<OkObjectResult>()
                .Which.Value.Should().BeAssignableTo<LineResponseDTO>()
                .Which.Should().BeEquivalentTo(line);
            _lineService.Verify(s => s.GetLineByLineIdAsync(line.Id), Times.Once);
        }

    }
}
