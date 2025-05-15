using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the outcome of a license validation attempt.
    /// REQ-LDM-LIC-002, REQ-LDM-LIC-005
    /// </summary>
    /// <param name="IsValid">Indicates whether the license is valid.</param>
    /// <param name="StatusMessage">A message describing the status of the validation.</param>
    /// <param name="ExpiryDate">The expiration date of the license, if applicable.</param>
    /// <param name="Features">A collection of features enabled by this license.</param>
    public record LicenseValidationResultDto(
        bool IsValid,
        string StatusMessage,
        DateTime? ExpiryDate,
        IEnumerable<string>? Features);
}