using System;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings for DICOM network operations managed or coordinated by the gateway.
    /// </summary>
    public class DicomGatewaySettings
    {
        /// <summary>
        /// Default timeout for individual DICOM operations (e.g., C-STORE, C-ECHO).
        /// </summary>
        public TimeSpan OperationTimeout { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Default number of retries for transient DICOM network failures.
        /// Used if a specific retry policy isn't defined for DICOM.
        /// </summary>
        public int DefaultRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Maximum number of concurrent DICOM operations initiated by this gateway instance.
        /// Helps prevent overwhelming the low-level DICOM client or network resources.
        /// </summary>
        public int MaxConcurrentClientOperations { get; set; } = 10;

        /// <summary>
        /// Identifier for the rate limiting policy specifically for DICOM operations, if any.
        /// Example: "DicomNetworkRateLimit"
        /// </summary>
        public string RateLimitResourceKey { get; set; } = "DicomNetwork";

        /// <summary>
        /// Default Calling Application Entity Title (AE Title) for this gateway when initiating DICOM operations.
        /// </summary>
        public string DefaultCallingAeTitle { get; set; } = "SVC_GATEWAY";

        /// <summary>
        /// Default port number this gateway might listen on if it were to act as an SCP (not its primary role, but could be for C-MOVE responses).
        /// Usually, for C-MOVE, the destination AE (this application) needs to be an SCP.
        /// </summary>
        public int DefaultScpPort { get; set; } = 11112; // Example port
    }
}