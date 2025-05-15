using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object representing the result of a license validation operation as exposed by the gateway.
/// </summary>
/// <param name="IsValid">Indicates whether the license is valid.</param>
/// <param name="Message">A message providing details about the validation status (e.g., "License valid", "License expired").</param>
/// <param name="ExpiryDate">The expiration date of the license, if applicable.</param>
/// <param name="EnabledFeatures">A list of features enabled by this license, if applicable.</param>
public record LicenseValidationResultDto(
    bool IsValid,
    string Message,
    DateTime? ExpiryDate,
    List<string>? EnabledFeatures
);