using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming DTOs are in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Defines the primary facade interface for the Service Integration Gateway, offering unified
    /// access to all integrated external services like Odoo, SMTP, Print, and DICOM network operations.
    /// </summary>
    public interface IExternalServiceCoordinator
    {
        /// <summary>
        /// Validates a license key against the Odoo Licensing API.
        /// </summary>
        /// <param name="licenseKey">The license key to validate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the validation result or error.</returns>
        Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends an email message via the configured SMTP service.
        /// </summary>
        /// <param name="emailMessage">The email message details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the email sending result or error.</returns>
        Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);

        /// <summary>
        /// Submits a print job to the Windows printing subsystem.
        /// </summary>
        /// <param name="printJob">The print job details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the print job submission result or error.</returns>
        Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DICOM C-STORE operation.
        /// </summary>
        /// <param name="request">The C-STORE request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the operation result or error.</returns>
        Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DICOM C-ECHO operation.
        /// </summary>
        /// <param name="request">The C-ECHO request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the operation result or error.</returns>
        Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DICOM C-FIND operation.
        /// </summary>
        /// <param name="request">The C-FIND request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the operation result or error, including found datasets.</returns>
        Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes a DICOM C-MOVE operation.
        /// </summary>
        /// <param name="request">The C-MOVE request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A GatewayResponse indicating the operation result or error.</returns>
        Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
    }
}