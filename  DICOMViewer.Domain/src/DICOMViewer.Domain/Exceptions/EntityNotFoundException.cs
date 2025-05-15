namespace TheSSS.DICOMViewer.Domain.Exceptions;
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException() { }
    public EntityNotFoundException(string message) : base(message) { }
    public EntityNotFoundException(string message, Exception inner) : base(message, inner) { }
}