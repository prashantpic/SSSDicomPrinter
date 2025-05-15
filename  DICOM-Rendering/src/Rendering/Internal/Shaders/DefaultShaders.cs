namespace TheSSS.DicomViewer.Rendering.Internal.Shaders
{
    public static class DefaultShaders
    {
        public const string WindowLevelSksl = @"
uniform float windowWidth;
        uniform float windowCenter;
        
        half4 main(float2 coord) {
            float value = sample(coord).r;
            float minValue = windowCenter - windowWidth/2;
            float maxValue = windowCenter + windowWidth/2;
            float result = clamp((value - minValue) / (maxValue - minValue), 0.0, 1.0);
            return half4(result, result, result, 1.0);
        }";

        public const string VoiLutSksl = @"
uniform sampler1D voiLut;
        
        half4 main(float2 coord) {
            float value = sample(coord).r;
            float mapped = texture(voiLut, value).r;
            return half4(mapped, mapped, mapped, 1.0);
        }";
    }
}