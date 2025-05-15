using System;

namespace TheSSS.DicomViewer.Common.Models
{
    public struct PixelSpacing : IEquatable<PixelSpacing>
    {
        public double RowSpacing { get; }
        public double ColumnSpacing { get; }

        public PixelSpacing(double rowSpacing, double columnSpacing)
        {
            RowSpacing = rowSpacing;
            ColumnSpacing = columnSpacing;
        }

        public override bool Equals(object obj)
        {
            return obj is PixelSpacing other && Equals(other);
        }

        public bool Equals(PixelSpacing other)
        {
            return RowSpacing.Equals(other.RowSpacing) && 
                   ColumnSpacing.Equals(other.ColumnSpacing);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(RowSpacing, ColumnSpacing);
        }

        public static bool operator ==(PixelSpacing left, PixelSpacing right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PixelSpacing left, PixelSpacing right)
        {
            return !(left == right);
        }
    }
}