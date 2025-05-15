using System;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for converting service-specific exceptions or error responses into a standardized ServiceErrorDto.
/// This interface is for a service that normalizes errors from different external services into a common format.
/// </summary>
public interface IUnifiedErrorHandlingService
{
    /// <summary>
    /// Handles an exception originating from an external service interaction, normalizing it into a ServiceErrorDto.
    /// </summary>
    /// <param name="exception">The exception caught.</param>
    /// <param name="serviceIdentifier">An identifier for the service where the error occurred (e.g., "OdooApi", "SmtpService").</param>
    /// <returns>A standardized ServiceErrorDto representation of the error.</returns>
    ServiceErrorDto HandleError(Exception exception, string serviceIdentifier);

    /// <summary>
    /// Handles a specific error response object returned by an external service, normalizing it into a ServiceErrorDto.
    /// This is for cases where the service returns a structured error object rather than throwing an exception.
    /// </summary>
    /// <param name="errorResponse">The service-specific error response object.</param>
    /// <param name="serviceIdentifier">An identifier for the service where the error occurred.</param>
    /// <returns>A standardized ServiceErrorDto representation of the error.</returns>
    ServiceErrorDto HandleErrorResponse(object errorResponse, string serviceIdentifier);
}