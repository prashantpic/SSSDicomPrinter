using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving application license status.
/// This abstracts the retrieval of license status from underlying services (e.g., LicensingOrchestrationService in REPO-APP-SERVICES).
/// </summary>
public interface ILicenseStatusAdapter
{
    /// <summary>
    /// Retrieves the current application license status.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="LicenseStatusInfoDto"/> representing the current license status.
    /// </returns>
    Task<LicenseStatusInfoDto> GetLicenseStatusAsync(CancellationToken cancellationToken);
}