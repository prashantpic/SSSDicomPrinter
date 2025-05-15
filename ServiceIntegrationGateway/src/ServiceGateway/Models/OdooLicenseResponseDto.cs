using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // For JsonPropertyName if needed for mapping

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for Odoo license validation responses, received by OdooApiAdapter.
    /// Represents the expected structure of the successful API response body from Odoo.
    /// Corresponds to REQ-LDM-LIC-004.
    /// </summary>
    public record OdooLicenseResponseDto
    {
        /// <summary>
        /// Indicates if the license key is valid according to Odoo.
        /// </summary>
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; init; }

        /// <summary>
        /// License expiry date, if applicable.
        /// </summary>
        [JsonPropertyName("expiry_date")]
        public DateTime? ExpiryDate { get; init; }

        /// <summary>
        /// List of features enabled by the license.
        /// </summary>
        [JsonPropertyName("features")]
        public List<string>? Features { get; init; }

        /// <summary>
        /// A message from Odoo, especially if validation failed (e.g., "License expired", "Key not found").
        /// This field is part of a successful API response that indicates a domain-level validation failure.
        /// </summary>
        [JsonPropertyName("status_message")]
        public string? StatusMessage { get; init; }

        /// <summary>
        /// Odoo might return a specific error code even in a 200 OK response for domain errors.
        /// </summary>
        [JsonPropertyName("error_code")]
        public string? ErrorCode { get; init; }

        // Parameterless constructor for deserialization
        public OdooLicenseResponseDto() { }

        // Constructor for manual creation (e.g., in tests)
        public OdooLicenseResponseDto(bool isValid, DateTime? expiryDate, List<string>? features, string? statusMessage, string? errorCode = null)
        {
            IsValid = isValid;
            ExpiryDate = expiryDate;
            Features = features;
            StatusMessage = statusMessage;
            ErrorCode = errorCode;
        }
    }
}