using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Mocks; // For MockSmtpService, MockOdooLicensingApiClient
// Assuming these interfaces and types exist in TheSSS.DicomViewer.Application
// namespace TheSSS.DicomViewer.Application
// {
//     public interface ISystemMonitoringService
//     {
//         Task CheckSystemHealthAndTriggerAlertsAsync();
//         void SimulateLowStorage(bool isLow); // For testing
//         void SimulatePacsConnectionStatus(bool isConnected); // For testing
//     }
//     // ILicensingOrchestrationService is defined in LicenseWorkflowTests.cs context
// }

namespace TheSSS.DicomViewer.IntegrationTests.Maintenance
{
    // Placeholders for services from TheSSS.DicomViewer.Application
    public interface ISystemMonitoringService
    {
        Task CheckSystemHealthAndTriggerAlertsAsync();
        void SimulateLowStorage(bool isLow);
        void SimulatePacsConnectionStatus(bool isConnected);
        void SimulateCriticalError(string errorMessage); // New method for generic error alerts
    }

    // ILicensingOrchestrationService placeholder is assumed to be available from other files or global usings.
    // If not, a minimal one would be:
    // public interface ILicensingOrchestrationService { Task CheckLicenseAndTriggerAlertsAsync(); }


    [Collection("SequentialIntegrationTests")]
    public class AlertSystemTests : IClassFixture<AppHostFixture>
    {
        private readonly AppHostFixture _appHostFixture;
        private readonly MockSmtpService _mockSmtpService;
        private readonly ILicensingOrchestrationService _licensingService; // Used to trigger license-related alerts
        private readonly ISystemMonitoringService _monitoringService; // Used to trigger system-related alerts
        private readonly MockOdooLicensingApiClient _mockOdooClient;

        public AlertSystemTests(AppHostFixture appHostFixture)
        {
            _appHostFixture = appHostFixture;
            _mockSmtpService = _appHostFixture.ServiceProvider.GetRequiredService<MockSmtpService>();
            _licensingService = _appHostFixture.ServiceProvider.GetRequiredService<ILicensingOrchestrationService>();
            _monitoringService = _appHostFixture.ServiceProvider.GetRequiredService<ISystemMonitoringService>();
            _mockOdooClient = _appHostFixture.ServiceProvider.GetRequiredService<MockOdooLicensingApiClient>();
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AlertGeneration_WhenLicenseApproachingExpiry_ShouldNotifyAdministrator()
        {
            // Arrange
            _mockSmtpService.ClearSentEmails();
            // Configure MockOdooLicensingApiClient to return a license that is approaching expiry
            // The ILicensingOrchestrationService should use this mock when checking.
            var expiryDate = DateTime.UtcNow.AddDays(10); // Example: 10 days to expiry
            _mockOdooClient.SetupValidationResponseForAnyKey(
                new Application.ILicensingApiClient.LicenseValidationResult { IsValid = true, ExpiryDate = expiryDate, IsApproachingExpiry = true });

            // Act
            // Trigger the process that checks license status and sends alerts.
            // This might be part of PerformStartupLicenseCheckAsync or a dedicated method.
            // For this test, let's assume PerformStartupLicenseCheckAsync also handles alert logic for expiry.
            await _licensingService.PerformStartupLicenseCheckAsync(); // Or a more specific method like CheckLicenseAndTriggerAlertsAsync()

            // Assert
            _mockSmtpService.SentEmails.Should().ContainSingle(email =>
                email.Subject.Contains("License Expiry Warning") &&
                email.Body.Contains("approaching expiration") &&
                email.Recipient.Contains("admin")); // Assuming admin email is configured
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AlertGeneration_WhenStorageSpaceLow_ShouldTriggerWarningAlert()
        {
            // Arrange
            _mockSmtpService.ClearSentEmails();
            // Simulate low storage condition. This might be done by configuring the ISystemMonitoringService
            // or mocking a dependency it uses (e.g., a service that checks disk space).
            // For this test, assume ISystemMonitoringService has a method to simulate this for testing.
            _monitoringService.SimulateLowStorage(true);

            // Act
            // Trigger the system monitoring check.
            await _monitoringService.CheckSystemHealthAndTriggerAlertsAsync();

            // Assert
            _mockSmtpService.SentEmails.Should().ContainSingle(email =>
                email.Subject.Contains("Low Storage Space Warning") &&
                email.Body.Contains("critically low") &&
                email.Recipient.Contains("admin"));
        }

        [Fact]
        [Trait("Category", "Integration")]
        public async Task AlertGeneration_WhenPacsConnectionLost_ShouldTriggerConnectivityAlert()
        {
            // Arrange
            _mockSmtpService.ClearSentEmails();
            // Simulate PACS connection loss.
            _monitoringService.SimulatePacsConnectionStatus(false); // false means connection lost

            // Act
            await _monitoringService.CheckSystemHealthAndTriggerAlertsAsync();

            // Assert
            _mockSmtpService.SentEmails.Should().ContainSingle(email =>
                email.Subject.Contains("PACS Connectivity Alert") &&
                email.Body.Contains("connection to PACS") &&
                email.Body.Contains("lost") &&
                email.Recipient.Contains("admin"));
        }
    }
}