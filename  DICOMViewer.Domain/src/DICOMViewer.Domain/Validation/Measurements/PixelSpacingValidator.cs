using FluentValidation;
using DICOMViewer.Domain.ValueObjects;

namespace DICOMViewer.Domain.Validation.Measurements
{
    public class PixelSpacingValidator : AbstractValidator<PixelSpacing>
    {
        public PixelSpacingValidator()
        {
            RuleFor(p => p.RowSpacing)
                .GreaterThan(0)
                .WithMessage("Row spacing must be greater than 0");
            
            RuleFor(p => p.ColumnSpacing)
                .GreaterThan(0)
                .WithMessage("Column spacing must be greater than 0");
        }
    }
}