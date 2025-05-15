using System;
using System.Collections.Generic;
using System.Text.Json.Serialization; // For potential custom mapping if Odoo uses different names

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for Odoo license validation responses, received by the OdooApiAdapter.
/// Represents the data structure received from the Odoo API after a license validation attempt.
/// </summary>
/// <param name="IsValid">Indicates if the license is considered valid by the Odoo API.</param>
/// <param name="Message">A message from the Odoo API describing the status of the license (e.g., "valid", "expired", "not_found").</param>
/// <param name="ExpiryDate">The expiration date of the license, if applicable.</param>
/// <param name="Features">A list of features enabled by this license, if provided by the API.</param>
/// <param name="ErrorCode">An optional error code from Odoo if the request failed or license is invalid in a specific way.</param>
/// <param name="RawData">Additional raw data or complex objects returned by Odoo, if any, for more detailed diagnostics or features.
/// Using Dictionary&lt;string, object&gt; as specified in initial SDS for flexibility.</param>
public record OdooLicenseResponseDto(
    // Mapped from Odoo's 'status' or a dedicated validity field
    [property: JsonPropertyName("is_valid")] // Example if Odoo uses snake_case
    bool IsValid,

    [property: JsonPropertyName("message")]
    string Message,

    [property: JsonPropertyName("expiry_date")]
    DateTime? ExpiryDate,

    [property: JsonPropertyName("enabled_features")]
    List<string>? Features,

    [property: JsonPropertyName("error_code")]
    string? ErrorCode = null,

    [property: JsonPropertyName("data")] // As per initial SDS for raw data
    Dictionary<string, object>? RawData = null
);