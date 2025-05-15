using System;
using System.Net;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here
using TheSSS.DICOMViewer.Integration.Adapters; // For custom adapter exceptions
using Polly.CircuitBreaker;
using Polly.Timeout;


namespace TheSSS.DICOMViewer.Integration.Services;

public class UnifiedErrorHandlingService : IUnifiedErrorHandlingService
{
    private readonly ILoggerAdapter _logger;

    private const string DefaultErrorCode = "UNEXPECTED_ERROR";
    private const string DefaultErrorMessage = "An unexpected error occurred.";

    public UnifiedErrorHandlingService(ILoggerAdapter logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ServiceErrorDto HandleError(Exception exception, string serviceIdentifier)
    {
        _logger.Error(exception, $"Handling exception from service '{serviceIdentifier}'. Exception Type: {exception.GetType().FullName}");

        string errorCode = DefaultErrorCode;
        string title = $"Error interacting with {serviceIdentifier}";
        string detail = exception.Message;

        switch (exception)
        {
            case ServiceIntegrationDisabledException ex:
                errorCode = "SERVICE_DISABLED";
                title = $"Service '{serviceIdentifier}' Disabled";
                detail = ex.Message;
                break;
            case PolicyNotFoundException ex:
                errorCode = "POLICY_CONFIG_ERROR";
                title = "Resilience Policy Configuration Error";
                detail = ex.Message;
                break;
            case RateLimitExceededException ex:
                errorCode = "RATE_LIMIT_EXCEEDED";
                title = "Rate Limit Exceeded";
                detail = $"Rate limit exceeded for {serviceIdentifier}. {ex.Message}";
                break;
            case CredentialRetrievalException ex:
                errorCode = "CREDENTIAL_ERROR";
                title = "Credential Retrieval Failed";
                detail = $"Could not retrieve credentials for {serviceIdentifier}. {ex.Message}";
                break;
            case TimeoutRejectedException ex: // Polly's specific timeout exception
            case TimeoutException exStd when !(exStd is TimeoutRejectedException): // Standard .NET Timeout
                errorCode = "SERVICE_TIMEOUT";
                title = $"Service '{serviceIdentifier}' Timed Out";
                detail = exStd.Message;
                break;
            case BrokenCircuitException ex:
                errorCode = "CIRCUIT_BROKEN";
                title = $"Service '{serviceIdentifier}' Unavailable";
                detail = $"The circuit breaker for {serviceIdentifier} is open. {ex.Message}";
                break;
            case OdooApiException ex:
                errorCode = $"ODOO_API_ERROR_{(int?)ex.StatusCode ?? 0}";
                title = "Odoo API Error";
                detail = $"Odoo API request failed. Status: {ex.StatusCode?.ToString() ?? "N/A"}. Message: {ex.Message}. Response: {Truncate(ex.ResponseContent, 200)}";
                break;
            case SmtpSendException ex:
                var innerSmtpEx = ex.InnerException as System.Net.Mail.SmtpException;
                errorCode = $"SMTP_SEND_ERROR_{(int?)innerSmtpEx?.StatusCode ?? 0}";
                title = "SMTP Send Error";
                detail = $"Failed to send email via SMTP. {ex.Message}. SMTP Status: {innerSmtpEx?.StatusCode.ToString() ?? "N/A"}";
                break;
            case PrintJobException ex:
                errorCode = "PRINT_JOB_ERROR";
                title = "Print Job Submission Error";
                detail = ex.Message;
                break;
            case DicomNetworkException ex:
                // Try to get more specific DICOM status from inner exception if available
                // This requires knowledge of the exceptions thrown by IDicomLowLevelClient
                string dicomStatusInfo = ""; 
                // Example: if IDicomLowLevelClient throws a specific exception type
                // if (ex.InnerException is DicomSpecificInfrastructureException dse) {
                //    dicomStatusInfo = $" DICOM Status: {dse.DicomStatus}";
                //    errorCode = $"DICOM_NETWORK_ERROR_{dse.DicomStatus}";
                // } else {
                errorCode = "DICOM_NETWORK_ERROR";
                // }
                title = "DICOM Network Error";
                detail = $"{ex.Message}{dicomStatusInfo}";
                break;
            case HttpRequestException ex:
                errorCode = $"HTTP_REQUEST_ERROR_{(int?)ex.StatusCode ?? 0}";
                title = "HTTP Request Error";
                detail = $"Network error during HTTP request to {serviceIdentifier}. Status: {ex.StatusCode?.ToString() ?? "N/A"}. {ex.Message}";
                break;
            case OperationCanceledException ex:
                errorCode = "OPERATION_CANCELLED";
                title = "Operation Cancelled";
                detail = $"The operation for {serviceIdentifier} was cancelled. {ex.Message}";
                break;
            default: // Handles any other exception
                errorCode = $"UNHANDLED_EXCEPTION_{exception.GetType().Name.ToUpperInvariant()}";
                title = $"Unexpected Error with {serviceIdentifier}";
                detail = $"An unexpected error of type {exception.GetType().Name} occurred: {exception.Message}";
                _logger.Warning($"Unhandled exception type {exception.GetType().Name} mapped to generic error for service '{serviceIdentifier}'.");
                break;
        }

        return new ServiceErrorDto(errorCode, title, detail, serviceIdentifier);
    }

    public ServiceErrorDto HandleErrorResponse(object errorResponse, string serviceIdentifier)
    {
        _logger.Warning($"Handling structured error response from service '{serviceIdentifier}'. Response Type: {errorResponse.GetType().FullName}");

        string errorCode = $"STRUCTURED_ERROR_{serviceIdentifier.ToUpperInvariant()}";
        string title = $"Service '{serviceIdentifier}' Reported an Error";
        string detail = "The service returned a structured error response.";

        // Example for Odoo if it returns a 200 OK with an error object in the body
        if (serviceIdentifier == "OdooApi" && errorResponse is OdooLicenseResponseDto odooResp)
        {
            // Assuming OdooLicenseResponseDto has fields that indicate an error even on 200 OK
            // For example, if odooResp.IsValid is false but the HTTP status was 200.
            if (!odooResp.IsValid && !string.IsNullOrWhiteSpace(odooResp.Message))
            {
                errorCode = "ODOO_LOGICAL_ERROR"; // Or extract a code from odooResp if available
                title = "Odoo Logical Error";
                detail = odooResp.Message;
            }
        }
        // Add specific handling for other services if they use this pattern

        return new ServiceErrorDto(errorCode, title, detail, serviceIdentifier);
    }

    private static string Truncate(string? value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
    }
}