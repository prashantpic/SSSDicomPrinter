using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for retrieving application license status (likely from REPO-APP-SERVICES or REPO-SEC-ORCH-001).
/// </summary>
public interface ILicenseStatusAdapter
{
    /// <summary>
    /// Retrieves the current application license status.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to the LicenseStatusInfoDto.</returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter cannot retrieve license status due to an issue with the underlying licensing service.
    /// </exception>
    Task<LicenseStatusInfoDto> GetLicenseStatusAsync(CancellationToken cancellationToken);
}