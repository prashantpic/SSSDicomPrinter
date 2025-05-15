using System;

namespace TheSSS.DICOMViewer.Integration.Models;

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
    /// Gets the data payload if the operation was successful.
    /// Will be default(T) (e.g., null for reference types) if IsSuccess is false.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// Gets the error information if the operation failed.
    /// Will be null if IsSuccess is true.
    /// </summary>
    public ServiceErrorDto? Error { get; init; }

    /// <summary>
    /// Private constructor to enforce creation via static factory methods.
    /// </summary>
    private GatewayResponse(bool isSuccess, T? data, ServiceErrorDto? error)
    {
        IsSuccess = isSuccess;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// Creates a successful gateway response with the specified data.
    /// </summary>
    /// <param name="data">The data payload for the successful response.</param>
    /// <returns>A new GatewayResponse instance indicating success.</returns>
    public static GatewayResponse<T> Success(T data)
    {
        return new GatewayResponse<T>(true, data, null);
    }

    /// <summary>
    /// Creates a failed gateway response with the specified error details.
    /// </summary>
    /// <param name="error">The error details for the failed response.</param>
    /// <returns>A new GatewayResponse instance indicating failure.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="error"/> is null.</exception>
    public static GatewayResponse<T> Failure(ServiceErrorDto error)
    {
        if (error == null)
        {
            throw new ArgumentNullException(nameof(error), "Error details must be provided for a failed response.");
        }
        return new GatewayResponse<T>(false, default, error);
    }
}