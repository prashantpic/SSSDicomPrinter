using System;

namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Specific exception thrown when an error occurs during cryptographic operations
    /// like encryption or decryption.
    /// REQ-7-017
    /// </summary>
    public class CryptographyOperationException : SecurityOrchestrationException
    {
        public CryptographyOperationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}