using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Sends emails to users.
    /// </summary>
    public interface IEmailNotifier
    {
        /// <summary>
        /// Sends an email based on a notification DTO.
        /// </summary>
        /// <param name="notificationDto">The notification details to include in the email.</param>
        /// <param name="cancellationToken">Cancellation token to stop sending if needed.</param>
        Task SendEmailAsync(NotificationDto notificationDto, CancellationToken cancellationToken);
    }
}
