using System.Collections.Generic;
using TheSSS.DicomViewer.Common.Models;

namespace TheSSS.DicomViewer.Common.Models.Measurements
{
    public class MeasurementInputParameters
    {
        public List<Point2D> Points { get; set; } = new List<Point2D>();
        public PixelSpacing? PixelSpacingInfo { get; set; }
    }
}