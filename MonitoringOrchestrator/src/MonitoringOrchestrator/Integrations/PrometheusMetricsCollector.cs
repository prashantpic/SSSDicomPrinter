using Prometheus;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using Microsoft.Extensions.Logging;

namespace TheSSS.DICOMViewer.Monitoring.Integrations;

public class PrometheusMetricsCollector
{
    private static readonly Gauge _storageFree = Metrics.CreateGauge(
        "dicom_storage_free_bytes", 
        "Free storage space in bytes");
        
    private static readonly Gauge _storageUsed = Metrics.CreateGauge(
        "dicom_storage_used_percent", 
        "Percentage of storage used");

    private static readonly Gauge _dbConnected = Metrics.CreateGauge(
        "database_connected", 
        "Database connection status");

    private static readonly Gauge _licenseValid = Metrics.CreateGauge(
        "license_valid", 
        "License validity status");

    private readonly ILogger<PrometheusMetricsCollector> _logger;

    public PrometheusMetricsCollector(ILogger<PrometheusMetricsCollector> logger)
    {
        _logger = logger;
    }

    public void UpdateMetrics(HealthReportDto report)
    {
        try
        {
            if (report.StorageHealth != null)
            {
                _storageFree.Set(report.StorageHealth.FreeSpaceBytes);
                _storageUsed.Set(report.StorageHealth.UsedPercentage);
            }

            if (report.DatabaseHealth != null)
            {
                _dbConnected.Set(report.DatabaseHealth.IsConnected ? 1 : 0);
            }

            if (report.LicenseStatus != null)
            {
                _licenseValid.Set(report.LicenseStatus.IsValid ? 1 : 0);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update Prometheus metrics");
        }
    }
}