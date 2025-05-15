namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Common Data Transfer Object for representing errors from any service integration.
/// Provides a standardized error structure for consistent error handling by clients of the gateway.
/// </summary>
/// <param name="Code">A structured, machine-readable error code representing the type of error
/// (e.g., "ODOO_API_ERROR_401", "NETWORK_TIMEOUT", "DICOM_SCP_FAILURE_A700").</param>
/// <param name="Message">A concise, human-readable message summarizing the error.
/// Suitable for display to users or for high-level logging.</param>
/// <param name="Details">More detailed information about the error, which may include technical specifics,
/// stack traces (though usually omitted for external DTOs), or original error messages from the underlying service.
/// This field can be used for debugging or more in-depth error analysis.</param>
/// <param name="SourceService">An identifier for the external service or component within the gateway
/// where the error originated (e.g., "OdooApiAdapter", "SmtpService", "DicomNetworkAdapter").</param>
public record ServiceErrorDto(
    string Code,
    string Message,
    string? Details, // Made nullable as details might not always be available or relevant
    string SourceService
);