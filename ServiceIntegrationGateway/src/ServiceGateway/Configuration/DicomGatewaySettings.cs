namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for DICOM network operations managed or coordinated by the gateway.
    /// Details retry policies, timeouts, and concurrency settings.
    /// </summary>
    public class DicomGatewaySettings
    {
        public int DefaultOperationTimeoutSeconds { get; set; } = 60;
        public int AssociationTimeoutSeconds { get; set; } = 30;

        // Concurrency settings REQ-DNSPI-009 (can be used by RateLimiter or DicomNetworkAdapter logic)
        public int MaxConcurrentCStoreOperations { get; set; } = 10;
        public int MaxConcurrentCFindOperations { get; set; } = 5;
        public int MaxConcurrentCMoveOperations { get; set; } = 5;
        public int MaxConcurrentCEchoOperations { get; set; } = 10;
        
        // Default retry settings for DICOM operations (can be overridden by global ResilienceSettings if needed)
        // These might be superseded or augmented by specific policies in ResilienceSettings.
        // public int DefaultRetryCount { get; set; } = 3;
        // public int DefaultRetryBackoffSeconds { get; set; } = 5;
    }
}