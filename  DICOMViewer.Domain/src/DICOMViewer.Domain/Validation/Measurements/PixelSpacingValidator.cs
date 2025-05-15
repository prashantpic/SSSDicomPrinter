using FluentValidation;
using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Validation.Measurements;

public class PixelSpacingValidator : AbstractValidator<PixelSpacing>
{
    public PixelSpacingValidator()
    {
        RuleFor(ps => ps.RowSpacing)
            .GreaterThan(0).WithMessage("Row spacing must be greater than zero");
        
        RuleFor(ps => ps.ColumnSpacing)
            .GreaterThan(0).WithMessage("Column spacing must be greater than zero");
    }
}