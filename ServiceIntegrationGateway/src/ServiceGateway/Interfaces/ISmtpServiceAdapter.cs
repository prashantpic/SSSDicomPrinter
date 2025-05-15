using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming DTOs are in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for adapters handling SMTP email sending operations,
    /// managing connections and email construction.
    /// </summary>
    public interface ISmtpServiceAdapter
    {
        /// <summary>
        /// Sends an email message via SMTP.
        /// </summary>
        /// <param name="emailMessage">The email message details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The specific result from the SMTP send operation.</returns>
        Task<SmtpSendResultDto> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);
    }
}