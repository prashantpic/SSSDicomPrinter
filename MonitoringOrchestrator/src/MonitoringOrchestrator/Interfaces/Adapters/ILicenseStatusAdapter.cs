namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for retrieving application license status.
/// Implementation is expected to be provided by REPO-APP-SERVICES (e.g., a LicensingOrchestrationService).
/// </summary>
public interface ILicenseStatusAdapter
{
    /// <summary>
    /// Retrieves the current application license status.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a LicenseStatusInfoDto detailing the license validity, expiry, etc.
    /// </returns>
    Task<LicenseStatusInfoDto> GetLicenseStatusAsync(CancellationToken cancellationToken);
}