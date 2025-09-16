using System.Net;
using FluentAssertions;
using LondonTubeNotifier.Core.Configuration;
using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;
using LondonTubeNotifier.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace UnitTests.Infrastructure.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<IEmailTemplateService> _templateServiceMock;
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly IOptions<SendGridSettings> _options;

        public EmailServiceTests()
        {
            _templateServiceMock = new Mock<IEmailTemplateService>();
            _loggerMock = new Mock<ILogger<EmailService>>();

            _options = Options.Create(new SendGridSettings
            {
                ApiKey = "dummy-api-key",
                FromEmail = "from@example.com",
                FromName = "TubeNotifier",
                DataResidency = "us"
            });
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenApiKeyIsMissing()
        {
            var options = Options.Create(new SendGridSettings { ApiKey = "", FromEmail = "a@b.com", FromName = "Name" });

            Action act = () => new EmailService(options, _loggerMock.Object, _templateServiceMock.Object);

            act.Should().Throw<InvalidOperationException>().WithMessage("SendGrid API key is missing.");
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenFromEmailIsMissing()
        {
            var options = Options.Create(new SendGridSettings { ApiKey = "key", FromEmail = "", FromName = "Name" });

            Action act = () => new EmailService(options, _loggerMock.Object, _templateServiceMock.Object);

            act.Should().Throw<InvalidOperationException>().WithMessage("SendGrid From Email is missing.");
        }

        [Fact]
        public void Constructor_ShouldThrow_WhenFromNameIsMissing()
        {
            var options = Options.Create(new SendGridSettings { ApiKey = "key", FromEmail = "a@b.com", FromName = "" });

            Action act = () => new EmailService(options, _loggerMock.Object, _templateServiceMock.Object);

            act.Should().Throw<InvalidOperationException>().WithMessage("SendGrid From Name is missing.");
        }

        [Fact]
        public async Task SendEmailAsync_ShouldSendEmailSuccessfully()
        {
            // Arrange
            var service = new TestableEmailService(_options, _loggerMock.Object, _templateServiceMock.Object);

            var notification = new NotificationDto
            {
                RecipientEmail = "to@example.com",
                RecipientName = "Recipient",
                LineUpdates = new LineStatusesDto
                {
                    LineName = "Victoria",
                    StatusDescriptions = new List<StatusesDto>
                    {
                        new StatusesDto { StatusSeverity = 5, StatusDescription = "Good Service" }
                    }
                }
            };

            _templateServiceMock.Setup(t => t.RenderAsync(It.IsAny<string>(), notification))
                .ReturnsAsync("<html>Rendered</html>");
            _templateServiceMock.Setup(t => t.GeneratePlainText(notification))
                .Returns("PlainText");

            service.SetSendEmailResponse(new Response(HttpStatusCode.Accepted, null, null));

            // Act
            await service.SendEmailAsync(notification, CancellationToken.None);

            // Assert
            service.LastSentMessage.Should().NotBeNull();
            service.LastSentMessage.Personalizations.First().Tos.First().Email.Should().Be("to@example.com");
            service.LastSentMessage.Personalizations.First().Subject.Should().Contain("Victoria");
        }

        [Fact]
        public async Task SendEmailAsync_ShouldThrow_WhenSendFails()
        {
            // Arrange
            var service = new TestableEmailService(_options, _loggerMock.Object, _templateServiceMock.Object);

            var notification = new NotificationDto
            {
                RecipientEmail = "to@example.com",
                RecipientName = "Recipient",
                LineUpdates = new LineStatusesDto
                {
                    LineName = "Victoria",
                    StatusDescriptions = new List<StatusesDto>
                    {
                        new StatusesDto { StatusSeverity = 5, StatusDescription = "Good Service" }
                    }
                }
            };

            _templateServiceMock.Setup(t => t.RenderAsync(It.IsAny<string>(), notification))
                .ReturnsAsync("<html>Rendered</html>");
            _templateServiceMock.Setup(t => t.GeneratePlainText(notification))
                .Returns("PlainText");

            service.SetSendEmailResponse(new Response(HttpStatusCode.BadRequest, null, null));

            // Act
            Func<Task> act = async () => await service.SendEmailAsync(notification, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("*Failed to send email*");
        }

        // Helper subclass to mock SendGridClient
        private class TestableEmailService : EmailService
        {
            public SendGridMessage? LastSentMessage;
            private Response? _response;

            public TestableEmailService(IOptions<SendGridSettings> options, ILogger<EmailService> logger, IEmailTemplateService emailTemplateService)
                : base(options, logger, emailTemplateService)
            {
            }

            public void SetSendEmailResponse(Response response)
            {
                _response = response;
            }

            protected override async Task<Response> SendEmailInternalAsync(SendGridMessage msg, CancellationToken cancellationToken)
            {
                LastSentMessage = msg;
                return await Task.FromResult(_response!);
            }
        }
    }
}
