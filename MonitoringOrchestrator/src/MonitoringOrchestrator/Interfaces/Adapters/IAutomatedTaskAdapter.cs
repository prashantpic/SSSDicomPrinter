namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IAutomatedTaskAdapter
{
    Task<IEnumerable<AutomatedTaskStatusInfoDto>> GetAutomatedTaskStatusesAsync(CancellationToken cancellationToken);
}