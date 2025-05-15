namespace TheSSS.DICOMViewer.Domain.Exceptions;

public class InvalidDicomIdentifierException : BusinessRuleViolationException
{
    public string InvalidValue { get; }

    public InvalidDicomIdentifierException(string message, string invalidValue) 
        : base(message) => InvalidValue = invalidValue;

    public InvalidDicomIdentifierException(string message, string invalidValue, Exception inner)
        : base(message, inner) => InvalidValue = invalidValue;
}