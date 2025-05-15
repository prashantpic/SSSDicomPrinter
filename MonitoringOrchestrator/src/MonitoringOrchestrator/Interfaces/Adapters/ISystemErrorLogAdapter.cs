namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface ISystemErrorLogAdapter
{
    Task<SystemErrorInfoSummaryDto> GetCriticalErrorSummaryAsync(TimeSpan lookbackPeriod, CancellationToken cancellationToken);
}