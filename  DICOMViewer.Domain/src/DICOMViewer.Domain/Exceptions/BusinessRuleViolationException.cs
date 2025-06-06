namespace TheSSS.DICOMViewer.Domain.Exceptions;

public class BusinessRuleViolationException : DomainException
{
    public BusinessRuleViolationException(string message) : base(message) { }
    public BusinessRuleViolationException(string message, Exception inner) : base(message, inner) { }
}