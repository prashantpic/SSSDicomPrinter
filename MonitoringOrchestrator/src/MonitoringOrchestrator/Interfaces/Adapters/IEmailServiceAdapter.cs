using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for interacting with the central EmailService (likely from REPO-APP-SERVICES or REPO-SVC-GATEWAY-001).
/// </summary>
public interface IEmailServiceAdapter
{
    /// <summary>
    /// Sends an email notification.
    /// </summary>
    /// <param name="recipients">List of recipient email addresses.</param>
    /// <param name="subject">The email subject line.</param>
    /// <param name="body">The email body (can be plain text or HTML, depending on the adapter's implementation).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task representing the asynchronous email sending operation.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.AlertingSystemException">
    /// Thrown if email sending fails due to issues with the email service or configuration.
    /// </exception>
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body, CancellationToken cancellationToken);
}