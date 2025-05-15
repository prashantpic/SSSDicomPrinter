namespace TheSSS.DICOMViewer.Application.Common.Exceptions;

public class NotFoundException : System.Exception
{
    public NotFoundException() { }
    public NotFoundException(string message) : base(message) { }
    public NotFoundException(string message, System.Exception inner) : base(message, inner) { }
    public NotFoundException(string entity, object key) 
        : base($"Entity \"{entity}\" ({key}) was not found") { }
}