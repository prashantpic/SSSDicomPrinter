using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here
using TheSSS.DICOMViewer.Integration.Adapters; // For ServiceIntegrationDisabledException etc.

namespace TheSSS.DICOMViewer.Integration.Coordinators;

/// <summary>
/// Implements IExternalServiceCoordinator, acting as the central facade.
/// It orchestrates calls to the various specialized service adapters (Odoo, SMTP, Print, DICOM),
/// normalizes responses and errors, and provides a unified interface to the application layer.
/// </summary>
public class ExternalServiceCoordinator : IExternalServiceCoordinator
{
    private readonly IOdooApiAdapter _odooApiAdapter;
    private readonly ISmtpServiceAdapter _smtpServiceAdapter;
    private readonly IWindowsPrintAdapter _windowsPrintAdapter;
    private readonly IDicomNetworkAdapter _dicomNetworkAdapter;
    private readonly IUnifiedErrorHandlingService _errorHandlingService;
    private readonly ILoggerAdapter _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExternalServiceCoordinator"/> class.
    /// </summary>
    /// <param name="odooApiAdapter">Adapter for Odoo API interactions.</param>
    /// <param name="smtpServiceAdapter">Adapter for SMTP service interactions.</param>
    /// <param name="windowsPrintAdapter">Adapter for Windows Print API interactions.</param>
    /// <param name="dicomNetworkAdapter">Adapter for DICOM network interactions.</param>
    /// <param name="errorHandlingService">Service for unified error handling.</param>
    /// <param name="logger">Adapter for logging.</param>
    public ExternalServiceCoordinator(
        IOdooApiAdapter odooApiAdapter,
        ISmtpServiceAdapter smtpServiceAdapter,
        IWindowsPrintAdapter windowsPrintAdapter,
        IDicomNetworkAdapter dicomNetworkAdapter,
        IUnifiedErrorHandlingService errorHandlingService,
        ILoggerAdapter logger)
    {
        _odooApiAdapter = odooApiAdapter ?? throw new ArgumentNullException(nameof(odooApiAdapter));
        _smtpServiceAdapter = smtpServiceAdapter ?? throw new ArgumentNullException(nameof(smtpServiceAdapter));
        _windowsPrintAdapter = windowsPrintAdapter ?? throw new ArgumentNullException(nameof(windowsPrintAdapter));
        _dicomNetworkAdapter = dicomNetworkAdapter ?? throw new ArgumentNullException(nameof(dicomNetworkAdapter));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating license validation for key: '{licenseKey.Substring(0, Math.Min(licenseKey.Length, 5))}...'."); // Log partial key for security
        try
        {
            var odooRequest = new OdooLicenseRequestDto(licenseKey);
            var odooResponse = await _odooApiAdapter.ValidateLicenseAsync(odooRequest, cancellationToken);

            // Map adapter-specific response to a standardized gateway DTO
            var resultDto = new LicenseValidationResultDto(
                odooResponse.IsValid,
                odooResponse.Message,
                odooResponse.ExpiryDate,
                odooResponse.Features
            );
            _logger.Information($"License validation coordinated successfully for key. Valid: {resultDto.IsValid}");
            return GatewayResponse<LicenseValidationResultDto>.Success(resultDto);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "OdooApi");
             return GatewayResponse<LicenseValidationResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating license validation for key.");
            var error = _errorHandlingService.HandleError(ex, "OdooApi");
            return GatewayResponse<LicenseValidationResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating email send to: {string.Join(", ", emailMessage.ToRecipients)}.");
        try
        {
            var smtpResult = await _smtpServiceAdapter.SendEmailAsync(emailMessage, cancellationToken);

            var resultDto = new EmailSendResultDto(
                smtpResult.IsSentSuccessfully,
                smtpResult.Message,
                smtpResult.MessageId
            );
            _logger.Information($"Email send coordinated successfully. Status: {resultDto.IsSentSuccessfully}");
            return GatewayResponse<EmailSendResultDto>.Success(resultDto);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "SmtpService");
             return GatewayResponse<EmailSendResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating email send to {string.Join(", ", emailMessage.ToRecipients)}.");
            var error = _errorHandlingService.HandleError(ex, "SmtpService");
            return GatewayResponse<EmailSendResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating print job submission for document: '{printJob.DocumentName ?? "Untitled"}'.");
        try
        {
            var printResult = await _windowsPrintAdapter.PrintDocumentAsync(printJob, cancellationToken);

            var resultDto = new PrintResultDto(
                printResult.IsSuccessful,
                printResult.Message,
                printResult.JobId
            );
            _logger.Information($"Print job submission coordinated successfully. Status: {resultDto.IsSuccessful}");
            return GatewayResponse<PrintResultDto>.Success(resultDto);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "WindowsPrint");
             return GatewayResponse<PrintResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating print job submission for document '{printJob.DocumentName ?? "Untitled"}'.");
            var error = _errorHandlingService.HandleError(ex, "WindowsPrint");
            return GatewayResponse<PrintResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating DICOM C-STORE to AE: '{request.TargetAe.AeTitle}'.");
        try
        {
            var dicomResult = await _dicomNetworkAdapter.SendCStoreAsync(request, cancellationToken);
            _logger.Information($"DICOM C-STORE coordinated successfully. Status: {dicomResult.IsSuccessful}");
            return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "DicomNetwork");
             return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating DICOM C-STORE to AE '{request.TargetAe.AeTitle}'.");
            var error = _errorHandlingService.HandleError(ex, "DicomNetwork");
            return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating DICOM C-ECHO to AE: '{request.TargetAe.AeTitle}'.");
        try
        {
            var dicomResult = await _dicomNetworkAdapter.SendCEchoAsync(request, cancellationToken);
            _logger.Information($"DICOM C-ECHO coordinated successfully. Status: {dicomResult.IsSuccessful}");
            return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "DicomNetwork");
             return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating DICOM C-ECHO to AE '{request.TargetAe.AeTitle}'.");
            var error = _errorHandlingService.HandleError(ex, "DicomNetwork");
            return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating DICOM C-FIND to AE: '{request.TargetAe.AeTitle}', Level: '{request.QueryLevel}'.");
        try
        {
            var dicomResult = await _dicomNetworkAdapter.SendCFindAsync(request, cancellationToken);
             _logger.Information($"DICOM C-FIND coordinated successfully. Status: {dicomResult.OperationResult.IsSuccessful}, Matches: {dicomResult.MatchingDatasets?.Count ?? 0}");
            return GatewayResponse<DicomCFindResultDto>.Success(dicomResult);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "DicomNetwork");
             return GatewayResponse<DicomCFindResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating DICOM C-FIND to AE '{request.TargetAe.AeTitle}'.");
            var error = _errorHandlingService.HandleError(ex, "DicomNetwork");
            return GatewayResponse<DicomCFindResultDto>.Failure(error);
        }
    }

    /// <inheritdoc/>
    public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default)
    {
        _logger.Information($"Coordinating DICOM C-MOVE from AE: '{request.TargetAe.AeTitle}' to Destination AE: '{request.DestinationAeTitle}'.");
        try
        {
            var dicomResult = await _dicomNetworkAdapter.SendCMoveAsync(request, cancellationToken);
            _logger.Information($"DICOM C-MOVE coordinated successfully. Status: {dicomResult.IsSuccessful}");
            return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
        }
        catch (ServiceIntegrationDisabledException disabledEx)
        {
             _logger.Warning(disabledEx.Message);
             var error = _errorHandlingService.HandleError(disabledEx, "DicomNetwork");
             return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error coordinating DICOM C-MOVE from AE '{request.TargetAe.AeTitle}' to '{request.DestinationAeTitle}'.");
            var error = _errorHandlingService.HandleError(ex, "DicomNetwork");
            return GatewayResponse<DicomOperationResultDto>.Failure(error);
        }
    }
}