using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record PixelAnonymizationTemplateId
    {
        public int Value { get; }

        public PixelAnonymizationTemplateId(int value)
        {
            if (value <= 0)
                throw new DomainException("Invalid Pixel Template ID");
            
            Value = value;
        }
    }
}