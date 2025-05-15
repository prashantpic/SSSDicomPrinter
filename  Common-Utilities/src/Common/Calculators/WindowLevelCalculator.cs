using System;
using TheSSS.DicomViewer.Common.Extensions;
using TheSSS.DicomViewer.Common.Models.Imaging;

namespace TheSSS.DicomViewer.Common.Calculators
{
    public static class WindowLevelCalculator
    {
        public static LookupTableOutput GenerateLookupTable(WindowLevelInput input)
        {
            var descriptor = input.PixelDescriptor;
            var windowWidth = input.WindowWidth;
            var windowCenter = input.WindowCenter;

            int bitsStored = descriptor.BitsStored;
            int maxRawValue = (1 << bitsStored) - 1;
            int minRawValue = descriptor.IsSigned ? -(1 << (bitsStored - 1)) : 0;

            double minModalityValue = minRawValue * descriptor.RescaleSlope + descriptor.RescaleIntercept;
            double maxModalityValue = maxRawValue * descriptor.RescaleSlope + descriptor.RescaleIntercept;

            int firstMapped = (int)Math.Floor(minModalityValue);
            int lastMapped = (int)Math.Ceiling(maxModalityValue);
            int entries = lastMapped - firstMapped + 1;

            var lut = new byte[entries];
            double windowStart = windowCenter - windowWidth / 2;
            double windowEnd = windowCenter + windowWidth / 2;

            for (int i = 0; i < entries; i++)
            {
                double modalityValue = firstMapped + i;
                double outputValue;

                if (modalityValue <= windowStart)
                {
                    outputValue = 0;
                }
                else if (modalityValue > windowEnd)
                {
                    outputValue = 255;
                }
                else
                {
                    outputValue = ((modalityValue - windowCenter) / windowWidth + 0.5) * 255;
                }

                lut[i] = (byte)outputValue.Clamp(0, 255);
            }

            return new LookupTableOutput
            {
                LutData = lut,
                NumberOfEntries = entries,
                FirstValueMapped = (short)firstMapped,
                BitsPerEntry = 8
            };
        }
    }
}