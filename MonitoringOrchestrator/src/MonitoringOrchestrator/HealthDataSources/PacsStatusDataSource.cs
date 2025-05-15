using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class PacsStatusDataSource : IHealthDataSource
{
    private readonly IPacsStatusAdapter _adapter;
    private readonly ILogger<PacsStatusDataSource> _logger;
    public string Name => "PACS";

    public PacsStatusDataSource(IPacsStatusAdapter adapter, ILogger<PacsStatusDataSource> logger)
        => (_adapter, _logger) = (adapter, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.GetAllPacsStatusesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PACS status check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}