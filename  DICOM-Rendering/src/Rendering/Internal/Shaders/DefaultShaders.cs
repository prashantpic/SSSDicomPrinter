namespace TheSSS.DicomViewer.Rendering.Internal.Shaders
{
    internal static class DefaultShaders
    {
        public const string WindowLevelSksl = @"
uniform float windowWidth;
uniform float windowCenter;
uniform float rescaleSlope;
uniform float rescaleIntercept;

vec4 main(vec2 coord) {
    vec4 src = sample(image, coord);
    float value = (src.r * rescaleSlope) + rescaleIntercept;
    float normalized = (value - (windowCenter - 0.5)) / (windowWidth - 1.0) + 0.5;
    return vec4(clamp(normalized, 0.0, 1.0));
}";

        public const string VoiLutSksl = @"
uniform float rescaleSlope;
uniform float rescaleIntercept;
uniform sampler1D lut;

vec4 main(vec2 coord) {
    vec4 src = sample(image, coord);
    float value = (src.r * rescaleSlope) + rescaleIntercept;
    return texture(lut, value);
}";
    }
}