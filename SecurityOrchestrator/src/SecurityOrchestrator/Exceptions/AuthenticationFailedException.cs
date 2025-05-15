using System;

namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Specific exception thrown when a user authentication attempt fails.
    /// REQ-7-006
    /// </summary>
    public class AuthenticationFailedException : SecurityOrchestrationException
    {
        public AuthenticationFailedException(string message)
            : base(message)
        {
        }

        public AuthenticationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}