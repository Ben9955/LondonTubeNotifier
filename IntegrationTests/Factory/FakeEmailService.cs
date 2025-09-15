using LondonTubeNotifier.Core.DTOs;
using LondonTubeNotifier.Core.ServiceContracts;

namespace IntegrationTests.Factory
{
    public class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(NotificationDto notificationDto, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }

}
