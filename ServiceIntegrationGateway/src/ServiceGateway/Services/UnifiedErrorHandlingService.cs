using System;
using System.Net.Http;
using System.Net.Mail;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming ILoggerAdapter namespace
using RestSharp; // For RestSharp specific exceptions if any, or to parse RestResponse content

// Placeholder for Dicom specific exceptions if they come from a specific library
// namespace ThirdParty.Dicom
// {
//     public class DicomNetworkException : Exception { public ushort DicomStatus {get; set;} /* ... */ }
// }


namespace TheSSS.DICOMViewer.Integration.Services
{
    public class UnifiedErrorHandlingService : IUnifiedErrorHandlingService
    {
        private readonly ILoggerAdapter<UnifiedErrorHandlingService> _logger;

        public UnifiedErrorHandlingService(ILoggerAdapter<UnifiedErrorHandlingService> logger)
        {
            _logger = logger;
        }

        public ServiceErrorDto HandleError(Exception exception, string serviceIdentifier)
        {
            _logger.LogError(exception, "Error handled from service: {ServiceIdentifier}. Exception Type: {ExceptionType}", serviceIdentifier, exception.GetType().Name);

            return exception switch
            {
                // Custom exceptions from adapters that might already contain ServiceErrorDto
                Adapters.OdooApiException odooEx => new ServiceErrorDto(odooEx.ErrorDetails.Code ?? "OdooFailure", odooEx.Message, odooEx.ToString(), serviceIdentifier),
                Adapters.DicomNetworkOperationException dicomEx => new ServiceErrorDto(dicomEx.ErrorDetails.Code ?? $"DICOM_{dicomEx.DicomStatus:X4}", dicomEx.Message, dicomEx.ToString(), serviceIdentifier),
                
                // Standard .NET exceptions
                HttpRequestException httpEx => new ServiceErrorDto(
                    httpEx.StatusCode?.ToString() ?? "NetworkError",
                    $"HTTP request error: {httpEx.Message}",
                    httpEx.ToString(),
                    serviceIdentifier),
                TaskCanceledException tcEx when tcEx.InnerException is TimeoutException => new ServiceErrorDto(
                    "Timeout",
                    "The operation timed out.",
                    tcEx.ToString(),
                    serviceIdentifier),
                TaskCanceledException tcEx => new ServiceErrorDto(
                     tcEx.CancellationToken.IsCancellationRequested ? "OperationCanceled" : "TaskCancelError",
                    "The operation was canceled.",
                    tcEx.ToString(),
                    serviceIdentifier),
                TimeoutException timeoutEx => new ServiceErrorDto(
                    "Timeout",
                    "The operation timed out.",
                    timeoutEx.ToString(),
                    serviceIdentifier),
                
                // Polly specific exceptions
                Polly.Timeout.TimeoutRejectedException trEx => new ServiceErrorDto(
                    "PollyTimeout",
                    "The operation governed by Polly TimeoutPolicy timed out.",
                    trEx.ToString(),
                    serviceIdentifier),
                Polly.CircuitBreaker.BrokenCircuitException bcEx => new ServiceErrorDto(
                    "CircuitBroken",
                    $"Circuit breaker is open: {bcEx.Message}",
                    bcEx.ToString(),
                    serviceIdentifier),
                Polly.CircuitBreaker.IsolatedCircuitException icEx => new ServiceErrorDto(
                    "CircuitIsolated",
                    $"Circuit breaker is isolated: {icEx.Message}",
                    icEx.ToString(),
                    serviceIdentifier),
                
                // Service specific exceptions
                SmtpException smtpEx => new ServiceErrorDto(
                    $"SmtpError_{smtpEx.StatusCode}",
                    $"SMTP error: {smtpEx.Message}",
                    smtpEx.ToString(),
                    serviceIdentifier),
                
                // Add other specific DICOM exceptions if IDicomLowLevelClient throws them
                // e.g. DicomNetworkException dicomEx => new ServiceErrorDto($"DICOM_{dicomEx.DicomStatus:X4}", dicomEx.Message, dicomEx.ToString(), serviceIdentifier),

                // RestSharp specific, if response was not successful (IsSuccessful is false)
                // This is typically caught in the adapter and response content is passed to HandleErrorResponse.
                // If RestClient throws directly, it might be an HttpRequestException or other.
                // For example, if `EnsureSuccessStatusCode()` was used implicitly or explicitly.

                // Generic fallback
                _ => new ServiceErrorDto(
                    "UnhandledException",
                    $"An unexpected error occurred in {serviceIdentifier}: {exception.Message}",
                    exception.ToString(),
                    serviceIdentifier)
            };
        }

        public ServiceErrorDto HandleErrorResponse(object errorResponse, string serviceIdentifier)
        {
            // This method needs to understand the structure of errorResponse for different services.
            // errorResponse could be a string (JSON, XML, plain text) or a deserialized object.
            _logger.LogWarning("Handling error response from service: {ServiceIdentifier}. Response: {ErrorResponse}", serviceIdentifier, errorResponse?.ToString());

            string errorCode = "ServiceError";
            string errorMessage = "The external service returned an error.";
            string errorDetails = errorResponse?.ToString() ?? "No details provided.";

            // Example: Odoo might return JSON like {"error": {"code": 200, "message": "Details"}}
            // Example: DICOM status codes might be part of a DTO (e.g. LowLevelDicomResponse from DicomNetworkAdapter)
            if (errorResponse is string responseString)
            {
                // Try to parse common error patterns, e.g. JSON
                // This is highly dependent on actual service responses.
                // For a robust solution, each adapter might parse its own errors and then call a more specific
                // version of HandleErrorResponse, or HandleError with a custom exception containing parsed details.
                // For now, a generic approach:
                errorMessage = $"Service {serviceIdentifier} returned an error: {responseString.Substring(0, Math.Min(responseString.Length, 100))}";
                errorDetails = responseString;
            }
            else if (errorResponse is Models.LowLevelDicomResponse dicomLowLevelError) // Assuming this DTO from IDicomLowLevelClient
            {
                errorCode = $"DICOM_{dicomLowLevelError.DicomStatus:X4}";
                errorMessage = $"DICOM operation failed with status {dicomLowLevelError.DicomStatus:X4}: {dicomLowLevelError.StatusMessage}";
                errorDetails = $"Target: {serviceIdentifier}, Status: {dicomLowLevelError.DicomStatus}, Message: {dicomLowLevelError.StatusMessage}";
            }
            // Add more specific handlers for other types of errorResponse objects if needed.

            return new ServiceErrorDto(errorCode, errorMessage, errorDetails, serviceIdentifier);
        }
    }
}