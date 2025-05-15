namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with the central EmailService.
/// Implementation is expected to be provided by REPO-APP-SERVICES or REPO-INFRA.
/// </summary>
public interface IEmailServiceAdapter
{
    /// <summary>
    /// Sends an email notification to a single recipient.
    /// </summary>
    /// <param name="recipient">The recipient's email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body (can be HTML or plain text, depending on implementation).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailAsync(string recipient, string subject, string body);

    /// <summary>
    /// Sends an email notification to multiple recipients.
    /// </summary>
    /// <param name="recipients">A list of recipient email addresses.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="body">The email body (can be HTML or plain text).</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body);
}