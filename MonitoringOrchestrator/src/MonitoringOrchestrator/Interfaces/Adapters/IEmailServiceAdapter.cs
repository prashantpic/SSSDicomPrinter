using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with a central EmailService.
/// This abstracts the actual email sending implementation.
/// </summary>
public interface IEmailServiceAdapter
{
    /// <summary>
    /// Sends an email notification.
    /// </summary>
    /// <param name="recipient">The email address of the recipient.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A task that represents the asynchronous email sending operation.</returns>
    Task SendEmailAsync(string recipient, string subject, string body);
}