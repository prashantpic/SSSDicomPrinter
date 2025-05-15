using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.HealthDataSources;

public class AutomatedTaskStatusDataSource : IHealthDataSource
{
    private readonly IAutomatedTaskAdapter _adapter;
    private readonly ILogger<AutomatedTaskStatusDataSource> _logger;
    public string Name => "AutomatedTasks";

    public AutomatedTaskStatusDataSource(IAutomatedTaskAdapter adapter, ILogger<AutomatedTaskStatusDataSource> logger)
        => (_adapter, _logger) = (adapter, logger);

    public async Task<object> GetHealthDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await _adapter.GetAutomatedTaskStatusesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Automated task status check failed");
            throw new DataSourceUnavailableException(Name, ex.Message, ex);
        }
    }
}