namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Generic Data Transfer Object for standardized responses from the IExternalServiceCoordinator.
    /// Indicates success/failure and carries data or error information.
    /// </summary>
    /// <typeparam name="T">The type of data returned on success.</typeparam>
    public record GatewayResponse<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public ServiceErrorDto? Error { get; init; }

        // Success factory
        public static GatewayResponse<T> Success(T data) =>
            new GatewayResponse<T> { IsSuccess = true, Data = data, Error = null };

        // Failure factory
        public static GatewayResponse<T> Failure(ServiceErrorDto error) =>
            new GatewayResponse<T> { IsSuccess = false, Data = default, Error = error };
        
        public static GatewayResponse<T> Failure(string code, string message, string details = "", string sourceService = "") =>
            new GatewayResponse<T> { IsSuccess = false, Data = default, Error = new ServiceErrorDto(code, message, details, sourceService) };
    }
}