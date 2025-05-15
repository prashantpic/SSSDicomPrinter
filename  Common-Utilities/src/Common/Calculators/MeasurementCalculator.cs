using System;
using System.Collections.Generic;
using TheSSS.DicomViewer.Common.Constants;
using TheSSS.DicomViewer.Common.Exceptions;
using TheSSS.DicomViewer.Common.Extensions;
using TheSSS.DicomViewer.Common.Models;
using TheSSS.DicomViewer.Common.Models.Measurements;

namespace TheSSS.DicomViewer.Common.Calculators
{
    public static class MeasurementCalculator
    {
        public static LengthMeasurement CalculateLength(MeasurementInputParameters parameters)
        {
            if (parameters.Points.Count != 2)
                throw new CalculationException("Exactly 2 points required for length measurement");

            var p1 = parameters.Points[0];
            var p2 = parameters.Points[1];
            var dx = p2.X - p1.X;
            var dy = p2.Y - p1.Y;
            var distancePixels = Math.Sqrt(dx * dx + dy * dy);

            if (parameters.PixelSpacingInfo.HasValue)
            {
                var spacing = parameters.PixelSpacingInfo.Value;
                var physicalDistance = distancePixels * Math.Sqrt(spacing.RowSpacing * spacing.ColumnSpacing);
                return new LengthMeasurement { Value = physicalDistance, Unit = MeasurementUnit.Millimeters };
            }

            return new LengthMeasurement { Value = distancePixels, Unit = MeasurementUnit.Pixels };
        }

        public static AngleMeasurement CalculateAngle(MeasurementInputParameters parameters)
        {
            if (parameters.Points.Count != 3)
                throw new CalculationException("Exactly 3 points required for angle measurement");

            var p1 = parameters.Points[0];
            var p2 = parameters.Points[1];
            var p3 = parameters.Points[2];

            var v1 = new Point2D(p1.X - p2.X, p1.Y - p2.Y);
            var v2 = new Point2D(p3.X - p2.X, p3.Y - p2.Y);

            var dot = (v1.X * v2.X) + (v1.Y * v2.Y);
            var mag1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            var mag2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);

            if (mag1.IsApproximately(0, NumericConstants.DoubleEpsilon) || 
                mag2.IsApproximately(0, NumericConstants.DoubleEpsilon))
                throw new CalculationException("Invalid vectors for angle calculation");

            var angleRad = Math.Acos(dot / (mag1 * mag2));
            return new AngleMeasurement { 
                Value = angleRad.ToDegrees(), 
                Unit = MeasurementUnit.Degrees 
            };
        }

        public static AreaMeasurement CalculatePolygonArea(MeasurementInputParameters parameters)
        {
            if (parameters.Points.Count < 3)
                throw new CalculationException("At least 3 points required for area calculation");

            double area = 0;
            var count = parameters.Points.Count;
            
            for (int i = 0; i < count; i++)
            {
                var j = (i + 1) % count;
                area += parameters.Points[i].X * parameters.Points[j].Y;
                area -= parameters.Points[i].Y * parameters.Points[j].X;
            }

            area = Math.Abs(area) / 2.0;

            if (parameters.PixelSpacingInfo.HasValue)
            {
                var spacing = parameters.PixelSpacingInfo.Value;
                var physicalArea = area * spacing.RowSpacing * spacing.ColumnSpacing;
                return new AreaMeasurement { Value = physicalArea, Unit = MeasurementUnit.SquareMillimeters };
            }

            return new AreaMeasurement { Value = area, Unit = MeasurementUnit.SquarePixels };
        }
    }
}