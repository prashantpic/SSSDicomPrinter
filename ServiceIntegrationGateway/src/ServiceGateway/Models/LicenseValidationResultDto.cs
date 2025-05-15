using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object representing the result of a license validation operation as exposed by the gateway.
    /// Corresponds to REQ-LDM-LIC-004.
    /// </summary>
    public record LicenseValidationResultDto
    {
        /// <summary>
        /// Indicates if the license is currently valid.
        /// </summary>
        public bool IsValid { get; init; }

        /// <summary>
        /// The expiration date of the license, if applicable and valid.
        /// </summary>
        public DateTime? ExpiryDate { get; init; }

        /// <summary>
        /// A list of features or modules enabled by this license.
        /// </summary>
        public List<string> Features { get; init; } = new List<string>();

        /// <summary>
        /// A message providing more context, especially if validation failed (e.g., "License expired", "Invalid key").
        /// This is for domain-level errors from Odoo, not transport errors.
        /// </summary>
        public string? StatusMessage { get; init; }

        public LicenseValidationResultDto(bool isValid, DateTime? expiryDate, List<string>? features, string? statusMessage = null)
        {
            IsValid = isValid;
            ExpiryDate = expiryDate;
            Features = features ?? new List<string>();
            StatusMessage = statusMessage;
        }
    }
}