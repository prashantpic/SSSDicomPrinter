using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Abstracts the mechanism for obtaining credentials (e.g., API keys, passwords) for external services, 
/// facilitating secure storage and seamless credential rotation. This interface is for securely retrieving 
/// and managing credentials for various external services, supporting credential rotation strategies.
/// </summary>
public interface ICredentialManager
{
    /// <summary>
    /// Retrieves credentials for a specified external service identifier.
    /// </summary>
    /// <param name="serviceIdentifier">A key identifying the service for which credentials are needed (e.g., "OdooApi", "SmtpService").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A ServiceCredentials object containing the retrieved credentials.</returns>
    Task<ServiceCredentials> GetCredentialsAsync(string serviceIdentifier, CancellationToken cancellationToken = default);
}