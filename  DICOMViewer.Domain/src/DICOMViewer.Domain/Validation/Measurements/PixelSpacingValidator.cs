using FluentValidation;
using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Validation.Measurements;

public class PixelSpacingValidator : AbstractValidator<PixelSpacing>
{
    public PixelSpacingValidator()
    {
        RuleFor(x => x.RowSpacing)
            .GreaterThan(0).WithMessage("Row spacing must be positive");
        
        RuleFor(x => x.ColumnSpacing)
            .GreaterThan(0).WithMessage("Column spacing must be positive");
    }
}