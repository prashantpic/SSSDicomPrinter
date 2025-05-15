using System;
using TheSSS.DICOMViewer.Integration.Interfaces; // For IUnifiedErrorHandlingService in helper methods

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Generic Data Transfer Object for standardized responses from the IExternalServiceCoordinator.
    /// Indicates success/failure and carries data or error information.
    /// </summary>
    /// <typeparam name="T">The type of the data payload on success.</typeparam>
    public record GatewayResponse<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; init; }

        /// <summary>
        /// Gets the data payload if the operation was successful. Null if not successful.
        /// </summary>
        public T? Result { get; init; } // Renamed from Data to Result to match SDS

        /// <summary>
        /// Gets the error information if the operation failed. Null if successful.
        /// </summary>
        public ServiceErrorDto? Error { get; init; }

        // Private constructor to enforce creation via static factory methods
        private GatewayResponse(bool isSuccess, T? result, ServiceErrorDto? error)
        {
            IsSuccess = isSuccess;
            Result = result;
            Error = error;
        }

        /// <summary>
        /// Creates a successful GatewayResponse with the specified data.
        /// </summary>
        /// <param name="result">The data payload.</param>
        /// <returns>A successful GatewayResponse.</returns>
        public static GatewayResponse<T> Success(T result)
        {
            return new GatewayResponse<T>(true, result, null);
        }

        /// <summary>
        /// Creates a failed GatewayResponse with the specified error details.
        /// </summary>
        /// <param name="error">The error information.</param>
        /// <returns>A failed GatewayResponse.</returns>
        public static GatewayResponse<T> Failure(ServiceErrorDto error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error), "Failure response must contain error details.");
            return new GatewayResponse<T>(false, default, error); // Use default for T data
        }

        /// <summary>
        /// Creates a failed GatewayResponse with error details derived from an exception.
        /// </summary>
        /// <param name="exception">The exception that caused the failure.</param>
        /// <param name="serviceIdentifier">Identifier of the service where the error occurred.</param>
        /// <param name="errorHandler">The error handling service to use for normalization.</param>
        /// <returns>A failed GatewayResponse.</returns>
         public static GatewayResponse<T> Failure(Exception exception, string serviceIdentifier, IUnifiedErrorHandlingService errorHandler)
         {
             if (exception == null) throw new ArgumentNullException(nameof(exception));
             if (string.IsNullOrEmpty(serviceIdentifier)) throw new ArgumentException("Service identifier cannot be null or empty.", nameof(serviceIdentifier));
             if (errorHandler == null) throw new ArgumentNullException(nameof(errorHandler));

             var errorDto = errorHandler.HandleError(exception, serviceIdentifier);
             return Failure(errorDto);
         }
    }
}