using System;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for converting service-specific exceptions or error responses 
/// into a standardized ServiceErrorDto.
/// </summary>
public interface IUnifiedErrorHandlingService
{
    /// <summary>
    /// Handles an exception, converting it into a standardized error DTO.
    /// </summary>
    /// <param name="exception">The exception that occurred.</param>
    /// <param name="serviceIdentifier">A string identifying the service where the error originated.</param>
    /// <returns>A standardized service error DTO.</returns>
    ServiceErrorDto HandleError(Exception exception, string serviceIdentifier);

    /// <summary>
    /// Handles a service-specific error response object, converting it into a standardized error DTO.
    /// </summary>
    /// <param name="errorResponse">The raw error response object from the external service.</param>
    /// <param name="serviceIdentifier">A string identifying the service where the error originated.</param>
    /// <returns>A standardized service error DTO.</returns>
    ServiceErrorDto HandleErrorResponse(object errorResponse, string serviceIdentifier);
}