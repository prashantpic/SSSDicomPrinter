using FluentValidation;

namespace TheSSS.DICOMViewer.Security.Validators
{
    // REQ-LDM-LIC-002: Basic validation for license key strings
    public class LicenseKeyValidator : AbstractValidator<string>
    {
        public LicenseKeyValidator()
        {
            RuleFor(key => key)
                .NotEmpty().WithMessage("License key cannot be empty.");
            // Add more specific format rules if applicable, e.g., length, character set, regex pattern.
            // Example:
            // .MinimumLength(20).WithMessage("License key must be at least 20 characters long.")
            // .Matches("^[A-Z0-9-]+$").WithMessage("License key can only contain uppercase letters, numbers, and hyphens.");
        }
    }
}