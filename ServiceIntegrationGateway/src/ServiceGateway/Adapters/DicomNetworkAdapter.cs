using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Infrastructure.Interfaces; // Assuming IDicomLowLevelClient is here
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here
using Dicom; // Assuming FO-DICOM or similar library types might be used internally by the low-level client

namespace TheSSS.DICOMViewer.Integration.Adapters;

public class DicomNetworkAdapter : IDicomNetworkAdapter
{
    private readonly IDicomLowLevelClient _dicomClient; // From REPO-INFRA
    private readonly DicomGatewaySettings _settings;
    private readonly IResiliencePolicyProvider _policyProvider;
    private readonly IRateLimiter _rateLimiter;
    private readonly ILoggerAdapter _logger;
    private readonly IAsyncPolicy _resiliencePolicy;
    private readonly ServiceGatewaySettings _gatewaySettings; // To check if DICOM is enabled
    private readonly SemaphoreSlim _concurrencyLimiter;

    public DicomNetworkAdapter(
        IDicomLowLevelClient dicomClient, 
        IOptions<DicomGatewaySettings> settings,
        IOptions<ServiceGatewaySettings> gatewaySettings,
        IResiliencePolicyProvider policyProvider,
        IRateLimiter rateLimiter,
        ILoggerAdapter logger)
    {
        _dicomClient = dicomClient ?? throw new ArgumentNullException(nameof(dicomClient));
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _gatewaySettings = gatewaySettings.Value ?? throw new ArgumentNullException(nameof(gatewaySettings));
        _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _resiliencePolicy = _policyProvider.GetPolicyAsync(_settings.PolicyKey);
        _concurrencyLimiter = new SemaphoreSlim(_settings.MaxConcurrentOperations > 0 ? _settings.MaxConcurrentOperations : Environment.ProcessorCount, 
                                                _settings.MaxConcurrentOperations > 0 ? _settings.MaxConcurrentOperations : Environment.ProcessorCount);
    }

    private async Task<TResult> ExecuteDicomOperationAsync<TResult>(
        string operationName,
        string targetAeDescription,
        Func<CancellationToken, Task<TResult>> dicomClientOperation,
        CancellationToken cancellationToken)
    {
        if (!_gatewaySettings.EnableDicomIntegration)
        {
            _logger.Information($"DICOM integration is disabled. Skipping {operationName}.");
            throw new ServiceIntegrationDisabledException("DICOM integration is disabled in settings.");
        }

        await _concurrencyLimiter.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_gatewaySettings.RateLimiting.EnableRateLimitingPerService)
            {
                 await _rateLimiter.AcquirePermitAsync(_settings.RateLimitResourceKey, cancellationToken).ConfigureAwait(false);
            }

            _logger.Information($"Executing DICOM {operationName} to {targetAeDescription}.");
            
            var result = await _resiliencePolicy.ExecuteAsync(
                ct => dicomClientOperation(ct), 
                cancellationToken)
                .ConfigureAwait(false);

            // Assuming result has IsSuccessful and StatusMessage properties (like DicomOperationResultDto)
            // This logging is generic; specific result types like DicomCFindResultDto might need custom logging.
            dynamic dynResult = result!; // Use dynamic for simplicity if properties are common
            _logger.Information($"DICOM {operationName} completed for {targetAeDescription}. Success: {dynResult.IsSuccessful}, Message: {dynResult.StatusMessage}");
            
            return result;
        }
        catch (Exception ex) when (!(ex is OperationCanceledException && cancellationToken.IsCancellationRequested)) // Don't wrap explicit cancellations
        {
            _logger.Error(ex, $"Error during DICOM {operationName} to {targetAeDescription}.");
            throw new DicomNetworkException($"Failed to execute DICOM {operationName} to {targetAeDescription}.", ex);
        }
        finally
        {
            _concurrencyLimiter.Release();
        }
    }

    public Task<DicomOperationResultDto> SendCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default)
    {
        var infraRequest = request.ToInfrastructureRequest(); // Assumes mapping extension exists
        return ExecuteDicomOperationAsync("C-STORE", request.TargetAe.AeTitle,
            ct => _dicomClient.SendCStoreAsync(infraRequest, ct), // This returns Infrastructure DTO
            cancellationToken)
            .ContinueWith(task => {
                if (task.IsFaulted) throw task.Exception!.GetBaseException(); // Propagate original exception
                return task.Result.ToGatewayResultDto(); // Map from Infrastructure DTO to Gateway DTO
            }, cancellationToken);
    }

    public Task<DicomOperationResultDto> SendCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default)
    {
        var infraRequest = request.ToInfrastructureRequest();
        return ExecuteDicomOperationAsync("C-ECHO", request.TargetAe.AeTitle,
            ct => _dicomClient.SendCEchoAsync(infraRequest, ct),
            cancellationToken)
             .ContinueWith(task => {
                if (task.IsFaulted) throw task.Exception!.GetBaseException();
                return task.Result.ToGatewayResultDto();
            }, cancellationToken);
    }

    public Task<DicomCFindResultDto> SendCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default)
    {
        var infraRequest = request.ToInfrastructureRequest();
        return ExecuteDicomOperationAsync("C-FIND", request.TargetAe.AeTitle,
            ct => _dicomClient.SendCFindAsync(infraRequest, ct), // This returns Infrastructure CFindResult DTO
            cancellationToken)
             .ContinueWith(task => {
                if (task.IsFaulted) throw task.Exception!.GetBaseException();
                return task.Result.ToGatewayCFindResultDto(); // Map from Infrastructure DTO to Gateway DTO
            }, cancellationToken);
    }

    public Task<DicomOperationResultDto> SendCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default)
    {
        var infraRequest = request.ToInfrastructureRequest();
        return ExecuteDicomOperationAsync("C-MOVE", $"{request.TargetAe.AeTitle} to {request.DestinationAeTitle}",
            ct => _dicomClient.SendCMoveAsync(infraRequest, ct),
            cancellationToken)
             .ContinueWith(task => {
                if (task.IsFaulted) throw task.Exception!.GetBaseException();
                return task.Result.ToGatewayResultDto();
            }, cancellationToken);
    }
}

// Custom exception for DICOM network errors
public class DicomNetworkException : Exception
{
    public DicomNetworkException(string message) : base(message) { }
    public DicomNetworkException(string message, Exception innerException) : base(message, innerException) { }
}

// --- Placeholder Mapping Extensions (Assume these exist or are implemented in a separate file/namespace) ---
// These map DTOs from ServiceGateway.Models to/from REPO-INFRA's DTOs.
public static class DicomRequestDtoMappingExtensions
{
    // To Infrastructure (Service Gateway -> REPO-INFRA)
    public static TheSSS.DICOMViewer.Infrastructure.Models.DicomCStoreRequest ToInfrastructureRequest(this DicomCStoreRequestDto dto)
    {
        return new TheSSS.DICOMViewer.Infrastructure.Models.DicomCStoreRequest(
            new TheSSS.DICOMViewer.Infrastructure.Models.DicomAETarget(dto.TargetAe.AeTitle, dto.TargetAe.Host, dto.TargetAe.Port),
            dto.DicomFilePaths, // Assuming structure matches
            dto.PreferredTransferSyntaxes);
    }

    public static TheSSS.DICOMViewer.Infrastructure.Models.DicomCEchoRequest ToInfrastructureRequest(this DicomCEchoRequestDto dto)
    {
        return new TheSSS.DICOMViewer.Infrastructure.Models.DicomCEchoRequest(
            new TheSSS.DICOMViewer.Infrastructure.Models.DicomAETarget(dto.TargetAe.AeTitle, dto.TargetAe.Host, dto.TargetAe.Port));
    }

    public static TheSSS.DICOMViewer.Infrastructure.Models.DicomCFindRequest ToInfrastructureRequest(this DicomCFindRequestDto dto)
    {
         // Assuming DicomDataset is compatible or a mapping exists
        return new TheSSS.DICOMViewer.Infrastructure.Models.DicomCFindRequest(
            new TheSSS.DICOMViewer.Infrastructure.Models.DicomAETarget(dto.TargetAe.AeTitle, dto.TargetAe.Host, dto.TargetAe.Port),
            dto.QueryLevel,
            dto.QueryKeys // Direct pass if type matches, else map Dicom.DicomDataset
            );
    }

    public static TheSSS.DICOMViewer.Infrastructure.Models.DicomCMoveRequest ToInfrastructureRequest(this DicomCMoveRequestDto dto)
    {
        return new TheSSS.DICOMViewer.Infrastructure.Models.DicomCMoveRequest(
             new TheSSS.DICOMViewer.Infrastructure.Models.DicomAETarget(dto.TargetAe.AeTitle, dto.TargetAe.Host, dto.TargetAe.Port),
             dto.DestinationAeTitle,
             dto.IdentifiersToMove // Direct pass if type matches
            );
    }

    // From Infrastructure (REPO-INFRA -> Service Gateway)
    public static DicomOperationResultDto ToGatewayResultDto(this TheSSS.DICOMViewer.Infrastructure.Models.DicomOperationResult infraResult)
    {
        return new DicomOperationResultDto(
            infraResult.IsSuccessful,
            infraResult.StatusMessage,
            infraResult.DicomStatusCode,
            infraResult.ErrorMessage
            );
    }
    
    public static DicomCFindResultDto ToGatewayCFindResultDto(this TheSSS.DICOMViewer.Infrastructure.Models.DicomCFindResult infraResult)
    {
        return new DicomCFindResultDto(
            infraResult.OperationResult.ToGatewayResultDto(),
            infraResult.MatchingDatasets // Direct pass if type matches, else map List<Dicom.DicomDataset>
            );
    }
}