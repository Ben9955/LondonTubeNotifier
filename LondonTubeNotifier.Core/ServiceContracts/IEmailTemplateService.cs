using LondonTubeNotifier.Core.DTOs;

namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Handles rendering email templates and generating plain text notifications.
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Renders a template using the provided model.
        /// </summary>
        Task<string> RenderAsync<T>(string templateName, T model);

        /// <summary>
        /// Generates a plain-text version of a notification.
        /// </summary>
        string GeneratePlainText(NotificationDto dto);
    }
}
