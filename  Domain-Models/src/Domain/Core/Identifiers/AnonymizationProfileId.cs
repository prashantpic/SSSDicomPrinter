using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record AnonymizationProfileId
    {
        public int Value { get; }

        public AnonymizationProfileId(int value)
        {
            if (value <= 0)
                throw new DomainException("Invalid Anonymization Profile ID");
            
            Value = value;
        }
    }
}