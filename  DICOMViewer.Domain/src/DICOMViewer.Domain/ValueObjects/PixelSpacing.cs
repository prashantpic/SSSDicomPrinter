namespace TheSSS.DICOMViewer.Domain.ValueObjects;
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
        var validator = new PixelSpacingValidator();
        var result = validator.Validate(spacing);
        
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return spacing;
    }
}