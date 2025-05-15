using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for sending email messages via SMTP, including handling attachments and SMTP-specific configurations like TLS.
/// This interface is for adapters handling SMTP email sending operations, managing connections and email construction.
/// </summary>
public interface ISmtpServiceAdapter
{
    /// <summary>
    /// Sends an email message via SMTP.
    /// </summary>
    /// <param name="emailMessage">The email message details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the SMTP send operation.</returns>
    Task<SmtpSendResultDto> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);
}