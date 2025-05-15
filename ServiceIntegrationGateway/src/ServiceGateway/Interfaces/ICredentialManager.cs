using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming ServiceCredentials DTO is in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for securely retrieving and managing credentials for various external services,
    /// supporting credential rotation strategies.
    /// </summary>
    public interface ICredentialManager
    {
        /// <summary>
        /// Retrieves credentials for a specific external service.
        /// </summary>
        /// <param name="serviceIdentifier">A unique identifier for the service requiring credentials (e.g., "OdooApi", "SmtpService").</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A ServiceCredentials object containing the requested credentials.</returns>
        /// <exception cref="CredentialManagementException">Thrown if credentials cannot be retrieved or are not found.</exception>
        Task<ServiceCredentials> GetCredentialsAsync(string serviceIdentifier, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Custom exception for credential management failures.
    /// </summary>
    [Serializable]
    public class CredentialManagementException : Exception
    {
        public string ServiceIdentifier { get; }

        public CredentialManagementException() : base() { ServiceIdentifier = string.Empty; }
        public CredentialManagementException(string serviceIdentifier) : base($"Credential management failed for service: {serviceIdentifier}")
        {
            ServiceIdentifier = serviceIdentifier;
        }
        public CredentialManagementException(string serviceIdentifier, string message) : base(message)
        {
            ServiceIdentifier = serviceIdentifier;
        }
        public CredentialManagementException(string serviceIdentifier, string message, Exception inner) : base(message, inner)
        {
            ServiceIdentifier = serviceIdentifier;
        }
        protected CredentialManagementException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context)
        {
            ServiceIdentifier = info.GetString(nameof(ServiceIdentifier)) ?? string.Empty;
        }

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ServiceIdentifier), ServiceIdentifier);
        }
    }
}