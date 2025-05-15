// Assuming ISystemMonitoringOrchestrationService, IStorageMonitorService, IPacsConnectivityMonitorService interfaces
// and PacsNodeStatus DTO are defined in TheSSS.DicomViewer.Application.Services
using TheSSS.DicomViewer.Application.Services;
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Mocks;

namespace TheSSS.DicomViewer.IntegrationTests.Maintenance;

[Collection("SequentialIntegrationTests")]
public class AlertSystemTests : IClassFixture<AppHostFixture>
{
    private readonly AppHostFixture _fixture;
    private readonly ISystemMonitoringOrchestrationService _monitoringService;
    private readonly MockSmtpService _mockSmtpService;
    private readonly MockOdooLicensingApiClient _mockLicensingClient;
    private readonly Mock<IStorageMonitorService> _mockStorageMonitor;
    private readonly Mock<IPacsConnectivityMonitorService> _mockPacsMonitor;
    private readonly string _adminEmail = "admin@dicomviewer.test"; // Example admin email

    public AlertSystemTests(AppHostFixture fixture)
    {
        _fixture = fixture;

        // Resolve the mock SMTP service and Licensing API client
        _mockSmtpService = (MockSmtpService)_fixture.ServiceProvider.GetRequiredService<ISmtpService>();
        _mockLicensingClient = (MockOdooLicensingApiClient)_fixture.ServiceProvider.GetRequiredService<ILicensingApiClient>();

        // For services like IStorageMonitorService and IPacsConnectivityMonitorService,
        // AppHostFixture needs to be configured to provide mocks for them if SystemMonitoringOrchestrationService depends on them.
        // We'll assume AppHostFixture has been set up to allow resolving these mocks.
        // If not, these mocks would need to be manually injected into a SystemMonitoringOrchestrationService instance.
        _mockStorageMonitor = _fixture.ServiceProvider.GetService<Mock<IStorageMonitorService>>() ?? new Mock<IStorageMonitorService>();
        _mockPacsMonitor = _fixture.ServiceProvider.GetService<Mock<IPacsConnectivityMonitorService>>() ?? new Mock<IPacsConnectivityMonitorService>();
        
        // Resolve the SystemMonitoringOrchestrationService itself.
        // If SystemMonitoringOrchestrationService takes dependencies on IStorageMonitorService/IPacsConnectivityMonitorService,
        // AppHostFixture must ensure these are resolved to the mocks above.
        _monitoringService = _fixture.ServiceProvider.GetRequiredService<ISystemMonitoringOrchestrationService>();


        // Clear state before each test
        _mockSmtpService.ClearSentEmails();
        _mockLicensingClient.Mock.Reset();
        _mockStorageMonitor.Reset();
        _mockPacsMonitor.Reset();
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-LDM-LIC-005")] // License expiry alert
    [Trait("Requirement", "REQ-7-024")]       // System alerts
    [Trait("Requirement", "REQ-LDM-MNT-011")]  // Alert dispatch
    public async Task AlertGeneration_WhenLicenseApproachingExpiry_ShouldNotifyAdministrator()
    {
        // Arrange
        // Simulate license approaching expiry. LicensingOrchestrationService would use ILicensingApiClient.
        _mockLicensingClient.Mock.Setup(m => m.ValidateLicenseAsync(It.IsAny<string>()))
            .ReturnsAsync(new LicenseValidationResult { IsValid = true, Status = "ActiveWarning", ExpiryDate = DateTime.UtcNow.AddDays(5).ToString("o") }); // Example near expiry

        // Act: Trigger the system check that would evaluate license status.
        // This assumes SystemMonitoringOrchestrationService or LicensingOrchestrationService has logic to check and raise alerts.
        await _monitoringService.PerformSystemChecksAsync(); // Assuming a method that includes license checks

        // Assert
        _mockSmtpService.SentEmails.Should().HaveCount(1, "One email alert should be sent for license warning.");
        var email = _mockSmtpService.SentEmails.First();
        email.Recipient.Should().Be(_adminEmail);
        email.Subject.Should().ContainEquivalentOf("License Expiry Warning", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase);
        email.Body.Should().ContainEquivalentOf("approaching expiry", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase);
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-7-024")]
    [Trait("Requirement", "REQ-LDM-MNT-011")]
    public async Task AlertGeneration_WhenStorageSpaceLow_ShouldTriggerWarningAlert()
    {
        // Arrange
        // Simulate low storage space through the IStorageMonitorService mock
        _mockStorageMonitor.Setup(s => s.GetStorageStatusAsync(It.IsAny<string>()))
            .ReturnsAsync(new StorageStatus { IsLowSpace = true, FreeSpacePercentage = 3.5, MonitoredPath = "C:\\DicomData" });
        // Ensure SystemMonitoringOrchestrationService is configured to use this mocked IStorageMonitorService.

        // Act
        await _monitoringService.PerformSystemChecksAsync(); // Assuming this includes storage checks

        // Assert
        _mockSmtpService.SentEmails.Should().HaveCount(1, "One email alert should be sent for low storage.");
        var email = _mockSmtpService.SentEmails.First();
        email.Recipient.Should().Be(_adminEmail);
        email.Subject.Should().ContainEquivalentOf("Low Storage Space Warning", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase);
        email.Body.Should().ContainEquivalentOf("running low on space", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase)
            .And.Contain("C:\\DicomData")
            .And.Contain("3.5%");
    }

    [Fact]
    [Trait("Category", "Maintenance")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-7-024")]
    [Trait("Requirement", "REQ-LDM-MNT-011")]
    public async Task AlertGeneration_WhenPacsConnectionLost_ShouldTriggerConnectivityAlert()
    {
        // Arrange
        var failedPacsNode = new PacsNodeStatus { NodeName = "PACS_MAIN", IsConnected = false, LastError = "Connection timed out" };
        _mockPacsMonitor.Setup(p => p.CheckAllNodesConnectivityAsync())
            .ReturnsAsync(new List<PacsNodeStatus> { failedPacsNode });
        // Ensure SystemMonitoringOrchestrationService uses this mocked IPacsConnectivityMonitorService.

        // Act
        await _monitoringService.PerformSystemChecksAsync(); // Assuming this includes PACS checks

        // Assert
        _mockSmtpService.SentEmails.Should().HaveCount(1, "One email alert should be sent for PACS connectivity issue.");
        var email = _mockSmtpService.SentEmails.First();
        email.Recipient.Should().Be(_adminEmail);
        email.Subject.Should().ContainEquivalentOf("PACS Connectivity Alert", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase);
        email.Body.Should().ContainEquivalentOf("Lost connection", FluentAssertions.Equivalency.StringComparisonOptions.IgnoreCase)
            .And.Contain("PACS_MAIN")
            .And.Contain("Connection timed out");
    }
}