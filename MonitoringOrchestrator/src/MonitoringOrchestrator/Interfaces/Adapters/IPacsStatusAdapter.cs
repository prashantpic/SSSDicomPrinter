namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IPacsStatusAdapter
{
    Task<IEnumerable<PacsConnectionInfoDto>> GetAllPacsStatusesAsync(CancellationToken cancellationToken);
}