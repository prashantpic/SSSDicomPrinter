using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Domain.Exceptions;

[Serializable]
public class DomainException : Exception
{
    public DomainException() { }
    public DomainException(string message) : base(message) { }
    public DomainException(string message, Exception inner) : base(message, inner) { }
    protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}