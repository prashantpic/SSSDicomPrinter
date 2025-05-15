using FluentValidation;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Validators
{
    // REQ-7-006: Validates the AuthenticationRequestDto
    public class AuthenticationRequestValidator : AbstractValidator<AuthenticationRequestDto>
    {
        public AuthenticationRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username must not be empty.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password must not be empty.");
            RuleFor(x => x.AuthType).NotEmpty().WithMessage("Authentication type (AuthType) must not be empty.");
        }
    }
}