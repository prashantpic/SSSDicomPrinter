using System;

namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Specific exception thrown when an error occurs during PHI masking operations,
    /// such as failure to apply rules.
    /// REQ-7-004
    /// </summary>
    public class PhiMaskingException : SecurityOrchestrationException
    {
        public PhiMaskingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}