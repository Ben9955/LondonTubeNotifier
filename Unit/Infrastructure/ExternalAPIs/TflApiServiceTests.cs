using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Infrastructure.Dtos;
using LondonTubeNotifier.Core.Exceptions;
using LondonTubeNotifier.Infrastructure.ExternalAPIs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq.Protected;
using Moq;

namespace UnitTests.Infrastructure.ExternalAPIs
{
    public class TflApiServiceTests
    {
        private readonly Mock<ILogger<TflApiService>> _loggerMock;
        private readonly TflSettings _settings;

        public TflApiServiceTests()
        {
            _loggerMock = new Mock<ILogger<TflApiService>>();
            _settings = new TflSettings
            {
                Modes = "tube",
                ApiKey = "test-api-key"
            };
        }

        private TflApiService CreateService(HttpMessageHandler handler)
        {
            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://api.tfl.gov.uk/")
            };
            var options = Options.Create(_settings);
            return new TflApiService(client, options, _loggerMock.Object);
        }

        [Fact]
        public async Task GetLinesStatusAsync_ShouldReturnDictionary_WhenApiReturnsValidData()
        {
            // Arrange
            var tflLineDtos = new HashSet<TflLineDto>
            {
                new TflLineDto
                {
                    LineId = "victoria",
                    LineStatuses = new List<TflStatusDto>
                    {
                        new TflStatusDto { StatusSeverity = 10, StatusSeverityDescription = "Good Service", Reason = null }
                    }
                }
            };

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(tflLineDtos)
                });

            var service = CreateService(handlerMock.Object);

            // Act
            var result = await service.GetLinesStatusAsync(CancellationToken.None);

            // Assert
            result.Should().ContainKey("victoria");
            result["victoria"].Should().ContainSingle();
            var status = result["victoria"].First();
            status.StatusSeverity.Should().Be(10);
            status.StatusDescription.Should().Be("Good Service");
        }

        [Fact]
        public async Task GetLinesStatusAsync_ShouldReturnEmptyDictionary_WhenApiReturnsEmptyData()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = JsonContent.Create(new HashSet<TflLineDto>())
                });

            var service = CreateService(handlerMock.Object);

            // Act
            var result = await service.GetLinesStatusAsync(CancellationToken.None);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLinesStatusAsync_ShouldThrowTflApiException_WhenStatusCodeIsNotSuccess()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server error")
                });

            var service = CreateService(handlerMock.Object);

            // Act
            Func<Task> act = async () => await service.GetLinesStatusAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<TflApiException>();
        }

        [Fact]
        public async Task GetLinesStatusAsync_ShouldThrowTflApiException_OnJsonException()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("InvalidJson")
                });

            var service = CreateService(handlerMock.Object);

            // Act
            Func<Task> act = async () => await service.GetLinesStatusAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<TflApiException>();
        }

        [Fact]
        public async Task GetLinesStatusAsync_ShouldThrowTflApiException_OnHttpRequestException()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            var service = CreateService(handlerMock.Object);

            // Act
            Func<Task> act = async () => await service.GetLinesStatusAsync(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<TflApiException>();
        }
    }
}
