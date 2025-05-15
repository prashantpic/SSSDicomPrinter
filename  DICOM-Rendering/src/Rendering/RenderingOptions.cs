using System.Collections.Generic;

namespace TheSSS.DicomViewer.Rendering
{
    public sealed class RenderingOptions
    {
        public double? WindowWidth { get; init; }
        public double? WindowCenter { get; init; }
        public string? VoiLutFunction { get; init; }
        public IReadOnlyList<VoiLutSequenceItem>? VoiLuts { get; init; }
        public bool Invert { get; init; }
        public float RotationDegrees { get; init; }
        public float ScaleFactor { get; init; } = 1.0f;
        public bool FlipHorizontal { get; init; }
        public bool FlipVertical { get; init; }
    }
}