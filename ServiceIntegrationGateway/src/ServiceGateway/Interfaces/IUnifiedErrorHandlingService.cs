using System;
using TheSSS.DICOMViewer.Integration.Models; // Assuming ServiceErrorDto is in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for a service that normalizes errors from different external services
    /// into a common format.
    /// </summary>
    public interface IUnifiedErrorHandlingService
    {
        /// <summary>
        /// Handles an exception thrown during an external service interaction, normalizing it into a ServiceErrorDto.
        /// Logs the original exception details.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        /// <param name="serviceIdentifier">A string identifying the service where the error occurred (e.g., "Odoo", "SMTP", "DICOM").</param>
        /// <returns>A standardized ServiceErrorDto.</returns>
        ServiceErrorDto HandleError(Exception exception, string serviceIdentifier);

        /// <summary>
        /// Handles an error response object received from an external service API, normalizing it into a ServiceErrorDto.
        /// Logs the original error response details.
        /// </summary>
        /// <param name="errorResponse">The service-specific error response object.</param>
        /// <param name="serviceIdentifier">A string identifying the service where the error occurred.</param>
        /// <returns>A standardized ServiceErrorDto.</returns>
        ServiceErrorDto HandleErrorResponse(object errorResponse, string serviceIdentifier);
    }
}