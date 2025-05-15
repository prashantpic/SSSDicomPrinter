namespace TheSSS.DICOMViewer.Domain.ValueObjects;

public record struct PixelSpacing(double RowSpacing, double ColumnSpacing)
{
    public static PixelSpacing Create(double row, double column)
    {
        if (row <= 0 || column <= 0)
            throw new BusinessRuleViolationException("Pixel spacing values must be positive");
        
        return new PixelSpacing(row, column);
    }

    public override string ToString() => $"{ColumnSpacing:F3}\\{RowSpacing:F3}";
}