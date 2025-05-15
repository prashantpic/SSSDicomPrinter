namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Base exception for all errors originating in the SecurityOrchestrator module.
    /// </summary>
    public class SecurityOrchestrationException : System.Exception
    {
        public SecurityOrchestrationException()
        {
        }

        public SecurityOrchestrationException(string message)
            : base(message)
        {
        }

        public SecurityOrchestrationException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
    }
}