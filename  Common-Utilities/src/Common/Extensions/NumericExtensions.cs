using TheSSS.DicomViewer.Common.Constants;

namespace TheSSS.DicomViewer.Common.Extensions
{
    public static class NumericExtensions
    {
        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static int Clamp(this int value, int min, int max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static bool IsApproximately(this double value, double target, double tolerance)
        {
            return Math.Abs(value - target) < tolerance;
        }

        public static double ToRadians(this double degrees)
        {
            return degrees * NumericConstants.PI / 180.0;
        }

        public static double ToDegrees(this double radians)
        {
            return radians * 180.0 / NumericConstants.PI;
        }
    }
}