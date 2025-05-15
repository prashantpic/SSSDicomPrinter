using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record HangingProtocolId
    {
        public int Value { get; }

        public HangingProtocolId(int value)
        {
            if (value <= 0)
                throw new DomainException("Invalid Hanging Protocol ID");
            
            Value = value;
        }
    }
}