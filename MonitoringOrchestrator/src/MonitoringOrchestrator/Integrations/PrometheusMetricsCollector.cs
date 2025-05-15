using Prometheus;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Integrations;

public class PrometheusMetricsCollector
{
    private static readonly Gauge _storageUsage = Metrics.CreateGauge(
        "dicom_storage_used_percent", 
        "Percentage of storage space used");
        
    private static readonly Gauge _dbStatus = Metrics.CreateGauge(
        "database_connected",
        "Database connection status");

    private static readonly Gauge _pacsStatus = Metrics.CreateGauge(
        "pacs_connected",
        "PACS node connectivity status",
        new GaugeConfiguration { LabelNames = new[] { "node" } });

    private static readonly Gauge _licenseStatus = Metrics.CreateGauge(
        "license_valid",
        "Application license validity");

    public void UpdateMetrics(HealthReportDto report)
    {
        _storageUsage.Set(report.StorageHealth?.UsedPercentage ?? -1);
        _dbStatus.Set(report.DatabaseHealth?.IsConnected == true ? 1 : 0);
        
        report.PacsConnections?.ForEach(p => 
            _pacsStatus.WithLabels(p.PacsNodeId).Set(p.IsConnected ? 1 : 0));
            
        _licenseStatus.Set(report.LicenseStatus?.IsValid == true ? 1 : 0);
    }
}