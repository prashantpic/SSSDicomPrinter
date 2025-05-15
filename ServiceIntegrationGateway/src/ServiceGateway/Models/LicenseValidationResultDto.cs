using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of a license validation operation as exposed by the gateway.
    /// </summary>
    public record LicenseValidationResultDto(
        bool IsValid,
        DateTime? ExpiryDate,
        List<string>? EnabledFeatures,
        string? Message
    );
}