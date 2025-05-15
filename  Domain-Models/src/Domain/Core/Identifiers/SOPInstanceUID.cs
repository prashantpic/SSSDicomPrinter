using System;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public record SOPInstanceUID(string Value)
    {
        public SOPInstanceUID() : this(string.Empty) { }
        
        public static SOPInstanceUID Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("SOP Instance UID cannot be empty");
            return new SOPInstanceUID(value);
        }
        
        public override string ToString() => Value;
    }
}