using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Abstracts the mechanism for obtaining credentials (e.g., API keys, passwords) 
/// for external services, facilitating secure storage and seamless credential rotation.
/// </summary>
public interface ICredentialManager
{
    /// <summary>
    /// Retrieves credentials for a specified service.
    /// </summary>
    /// <param name="serviceIdentifier">A unique identifier for the service requiring credentials.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the service credentials.</returns>
    Task<ServiceCredentials> GetCredentialsAsync(string serviceIdentifier, CancellationToken cancellationToken = default);
}