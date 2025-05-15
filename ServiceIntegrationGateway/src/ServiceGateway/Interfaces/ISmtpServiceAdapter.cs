using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for sending email messages via SMTP, 
/// including handling attachments and SMTP-specific configurations like TLS.
/// </summary>
public interface ISmtpServiceAdapter
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="emailMessage">The email DTO containing message details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the SMTP send operation.</returns>
    Task<SmtpSendResultDto> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);
}