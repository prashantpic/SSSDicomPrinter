using System;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public record PatientId(string Value)
    {
        public PatientId() : this(string.Empty) { }
        
        public static PatientId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Patient ID cannot be empty");
            return new PatientId(value);
        }
        
        public override string ToString() => Value;
    }
}