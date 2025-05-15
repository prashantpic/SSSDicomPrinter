using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for the ApiCoordinator component. It provides a single, simplified interface to external services, 
/// abstracting the complexities of individual adapters, resilience, and error handling.
/// This interface offers unified access to all integrated external services like Odoo, SMTP, Print, and DICOM network operations.
/// </summary>
public interface IExternalServiceCoordinator
{
    /// <summary>
    /// Validates a license key against the Odoo Licensing API.
    /// </summary>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the validation.</returns>
    Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email message via the configured SMTP service.
    /// </summary>
    /// <param name="emailMessage">The email message details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the email sending operation.</returns>
    Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits a print job to the Windows Print API.
    /// </summary>
    /// <param name="printJob">The print job details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the print submission.</returns>
    Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-STORE operation.
    /// </summary>
    /// <param name="request">The C-STORE request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the C-STORE operation.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-ECHO operation.
    /// </summary>
    /// <param name="request">The C-ECHO request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the C-ECHO operation.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-FIND operation.
    /// </summary>
    /// <param name="request">The C-FIND request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the C-FIND operation, including matched datasets.</returns>
    Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-MOVE operation.
    /// </summary>
    /// <param name="request">The C-MOVE request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A GatewayResponse indicating the result of the C-MOVE operation.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
}