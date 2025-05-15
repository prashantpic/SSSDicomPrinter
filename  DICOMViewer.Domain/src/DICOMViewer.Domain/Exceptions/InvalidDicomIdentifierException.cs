namespace DICOMViewer.Domain.Exceptions
{
    public class InvalidDicomIdentifierException : BusinessRuleViolationException
    {
        public string InvalidIdentifier { get; }
        
        public InvalidDicomIdentifierException(string identifier, string message) 
            : base($"Invalid DICOM identifier '{identifier}': {message}")
        {
            InvalidIdentifier = identifier;
        }
    }
}