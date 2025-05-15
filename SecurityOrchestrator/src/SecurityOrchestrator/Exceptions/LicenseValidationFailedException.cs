using System;

namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Specific exception thrown when software license validation or activation fails.
    /// REQ-LDM-LIC-002, REQ-LDM-LIC-005
    /// </summary>
    public class LicenseValidationFailedException : SecurityOrchestrationException
    {
        public LicenseValidationFailedException(string message)
            : base(message)
        {
        }

        public LicenseValidationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}