using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record UserSettingId
    {
        public int Value { get; }

        public UserSettingId(int value)
        {
            if (value <= 0)
                throw new DomainException("Invalid User Setting ID");
            
            Value = value;
        }
    }
}