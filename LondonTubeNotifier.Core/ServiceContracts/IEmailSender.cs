namespace LondonTubeNotifier.Core.ServiceContracts
{
    /// <summary>
    /// Provides a lower-level interface to send emails.
    /// </summary>
    public interface IEmailSender
    {
        /// <summary>
        /// Sends a single email.
        /// </summary>
        /// <param name="toEmail">Recipient email address.</param>
        /// <param name="toName">Recipient name.</param>
        /// <param name="subject">Email subject.</param>
        /// <param name="plainText">Plain text version of the email.</param>
        /// <param name="html">HTML version of the email.</param>
        /// <param name="ct">Cancellation token.</param>
        Task SendAsync(
            string toEmail,
            string toName,
            string subject,
            string plainText,
            string html,
            CancellationToken ct);
    }
}
