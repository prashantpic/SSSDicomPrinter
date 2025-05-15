using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for Odoo license validation responses, received by OdooApiAdapter.
    /// Represents the data structure received from the Odoo API after a license validation attempt.
    /// This is a general representation; actual fields depend on Odoo's specific API response.
    /// </summary>
    public record OdooLicenseResponseDto
    {
        [JsonPropertyName("is_valid")]
        public bool IsValid { get; init; }

        [JsonPropertyName("expiry_date")]
        public DateTime? ExpiryDate { get; init; }

        [JsonPropertyName("enabled_features")]
        public List<string>? EnabledFeatures { get; init; }

        [JsonPropertyName("message")]
        public string? Message { get; init; } // General message from Odoo

        [JsonPropertyName("error_code")]
        public string? ErrorCode { get; init; } // Specific error code if validation failed

        [JsonPropertyName("error_details")]
        public string? ErrorDetails { get; init; } // More details about the error

        // Catch-all for any other properties Odoo might send
        [JsonExtensionData]
        public Dictionary<string, object>? AdditionalData { get; init; }
    }
}