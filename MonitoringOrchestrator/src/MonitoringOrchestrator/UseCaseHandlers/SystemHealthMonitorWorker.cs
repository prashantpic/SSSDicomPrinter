using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers
{
    public class SystemHealthMonitorWorker : BackgroundService
    {
        private readonly ILogger<SystemHealthMonitorWorker> _logger;
        private readonly MonitoringOptions _monitoringOptions;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SystemHealthMonitorWorker(
            ILogger<SystemHealthMonitorWorker> logger,
            IOptions<MonitoringOptions> monitoringOptions,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _monitoringOptions = monitoringOptions.Value;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("System Health Monitor Worker started");

            if (!_monitoringOptions.IsMonitoringEnabled)
            {
                _logger.LogWarning("Monitoring is disabled in configuration");
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var aggregator = scope.ServiceProvider.GetRequiredService<HealthAggregationService>();
                        var evaluator = scope.ServiceProvider.GetRequiredService<AlertEvaluationService>();

                        var report = await aggregator.AggregateHealthDataAsync(stoppingToken);
                        await evaluator.EvaluateHealthReportAsync(report, stoppingToken);
                    }
                }
                catch (TaskCanceledException)
                {
                    _logger.LogInformation("Monitoring worker stopping due to cancellation");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Critical error in monitoring cycle");
                }

                await Task.Delay(_monitoringOptions.SystemHealthCheckInterval, stoppingToken);
            }

            _logger.LogInformation("System Health Monitor Worker stopped");
        }
    }
}