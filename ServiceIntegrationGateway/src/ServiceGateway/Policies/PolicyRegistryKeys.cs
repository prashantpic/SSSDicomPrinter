using System;

namespace TheSSS.DICOMViewer.Integration.Policies
{
    /// <summary>
    /// Defines static constant string keys used to identify and retrieve specific Polly policies
    /// from the IResiliencePolicyProvider or a PolicyRegistry.
    /// </summary>
    public static class PolicyRegistryKeys
    {
        /// <summary>
        /// Default Retry policy key.
        /// </summary>
        public static readonly string DefaultRetryPolicyKey = "DefaultRetry";

        /// <summary>
        /// Default Circuit Breaker policy key.
        /// </summary>
        public static readonly string DefaultCircuitBreakerPolicyKey = "DefaultCircuitBreaker";

        /// <summary>
        /// Default Timeout policy key.
        /// </summary>
        public static readonly string DefaultTimeoutPolicyKey = "DefaultTimeout";

        /// <summary>
        /// Key for the general Odoo API policy chain.
        /// </summary>
        public static readonly string OdooApiPolicy = "OdooApiPolicy";

        /// <summary>
        /// Key for the general SMTP service policy chain.
        /// </summary>
        public static readonly string SmtpPolicy = "SmtpPolicy";

        /// <summary>
        /// Key for the general Windows Print policy chain.
        /// </summary>
        public static readonly string WindowsPrintPolicy = "WindowsPrintPolicy";

        /// <summary>
        /// Key for the general DICOM network operations policy chain.
        /// </summary>
        public static readonly string DicomPolicy = "DicomPolicy";

        // Example of a more specific key if needed
        // public static readonly string DicomCStorePolicy = "DicomCStorePolicy";
    }
}