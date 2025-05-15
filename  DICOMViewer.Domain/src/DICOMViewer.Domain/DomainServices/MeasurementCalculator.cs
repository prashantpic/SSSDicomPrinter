using System;
using System.Collections.Generic;
using System.Linq;
using DICOMViewer.Domain.ValueObjects;
using DICOMViewer.Domain.Exceptions;

namespace DICOMViewer.Domain.DomainServices
{
    public class MeasurementCalculator
    {
        public Length CalculateLength(ImageCoordinates start, ImageCoordinates end, PixelSpacing spacing)
        {
            ValidateSpacing(spacing);
            
            var dx = (end.X - start.X) * spacing.ColumnSpacing;
            var dy = (end.Y - start.Y) * spacing.RowSpacing;
            var length = Math.Sqrt(dx * dx + dy * dy);
            
            return Length.Create(length);
        }

        public Angle CalculateAngle(ImageCoordinates p1, ImageCoordinates p2, ImageCoordinates p3, PixelSpacing spacing)
        {
            ValidateSpacing(spacing);
            
            var v1 = ToVector(p1, p2, spacing);
            var v2 = ToVector(p3, p2, spacing);
            
            var dot = v1.X * v2.X + v1.Y * v2.Y;
            var mag1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            var mag2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);
            
            var angle = Math.Acos(dot / (mag1 * mag2)) * (180 / Math.PI);
            return Angle.Create(angle);
        }

        public Area CalculateArea(IEnumerable<ImageCoordinates> vertices, PixelSpacing spacing)
        {
            ValidateSpacing(spacing);
            var points = vertices.ToList();
            if (points.Count < 3)
                throw new MeasurementCalculationException("At least 3 vertices required for area calculation");

            double area = 0;
            for (int i = 0; i < points.Count; i++)
            {
                var j = (i + 1) % points.Count;
                area += points[i].X * points[j].Y;
                area -= points[i].Y * points[j].X;
            }
            area = Math.Abs(area) * 0.5 * spacing.RowSpacing * spacing.ColumnSpacing;
            
            return Area.Create(area);
        }

        private static void ValidateSpacing(PixelSpacing spacing)
        {
            if (spacing.RowSpacing <= 0 || spacing.ColumnSpacing <= 0)
                throw new MeasurementCalculationException("Invalid pixel spacing values");
        }

        private (double X, double Y) ToVector(ImageCoordinates from, ImageCoordinates to, PixelSpacing spacing)
        {
            return (
                (to.X - from.X) * spacing.ColumnSpacing,
                (to.Y - from.Y) * spacing.RowSpacing
            );
        }
    }
}