using DICOMViewer.Domain.Validation.Measurements;

namespace DICOMViewer.Domain.ValueObjects
{
    public readonly record struct PixelSpacing
    {
        public double RowSpacing { get; }
        public double ColumnSpacing { get; }

        private PixelSpacing(double rowSpacing, double columnSpacing)
        {
            RowSpacing = rowSpacing;
            ColumnSpacing = columnSpacing;
        }

        public static PixelSpacing Create(double rowSpacing, double columnSpacing)
        {
            var spacing = new PixelSpacing(rowSpacing, columnSpacing);
            new PixelSpacingValidator().ValidateAndThrow(spacing);
            return spacing;
        }
    }
}