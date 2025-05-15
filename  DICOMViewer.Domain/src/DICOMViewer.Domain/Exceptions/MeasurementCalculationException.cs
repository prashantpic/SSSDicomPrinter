namespace TheSSS.DICOMViewer.Domain.Exceptions;
public class MeasurementCalculationException : DomainException
{
    public MeasurementCalculationException(string message) : base(message) { }
    public MeasurementCalculationException(string message, Exception inner) : base(message, inner) { }
}