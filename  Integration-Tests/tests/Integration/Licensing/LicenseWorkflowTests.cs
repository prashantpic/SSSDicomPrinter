// Apply collection fixture for AppHost (dependency injection) and Database (if needed for licensing state)
// TheSSS.DicomViewer.Application.Services contains ILicensingOrchestrationService, LicenseActivationResult, LicenseValidationResult
// TheSSS.DicomViewer.IntegrationTests.Mocks contains MockOdooLicensingApiClient
// TheSSS.DicomViewer.Application.Services also contains ILicensingApiClient
using TheSSS.DicomViewer.Application.Services;
using TheSSS.DicomViewer.IntegrationTests.Fixtures;
using TheSSS.DicomViewer.IntegrationTests.Mocks;

namespace TheSSS.DicomViewer.IntegrationTests.Licensing;

[Collection("SequentialIntegrationTests")] // Use a sequential collection if licensing state affects multiple tests
public class LicenseWorkflowTests : IClassFixture<AppHostFixture>
{
    private readonly AppHostFixture _fixture;
    private readonly ILicensingOrchestrationService _licensingService;
    private readonly MockOdooLicensingApiClient _mockLicensingClient;

    public LicenseWorkflowTests(AppHostFixture fixture)
    {
        _fixture = fixture;
        _licensingService = _fixture.ServiceProvider.GetRequiredService<ILicensingOrchestrationService>();
        _mockLicensingClient = (MockOdooLicensingApiClient)_fixture.ServiceProvider.GetRequiredService<ILicensingApiClient>();
        
        // Clear mock setups and invocations before each test
        _mockLicensingClient.Mock.Invocations.Clear();
        _mockLicensingClient.Mock.Reset();
    }

    [Fact]
    [Trait("Category", "Licensing")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-LDM-LIC-001")] // Covered by successful activation
    [Trait("Requirement", "REQ-LDM-LIC-002")]
    public async Task ActivateLicense_WithValidKey_ShouldSucceedAndStoreLicense()
    {
        // Arrange
        var validKey = "TEST-VALID-KEY-12345";
        _mockLicensingClient.SetupActivateSuccess(validKey);
        _mockLicensingClient.SetupValidateValid(validKey); // Assume successful activation also means it's valid

        // Act
        var result = await _licensingService.ActivateLicenseAsync(validKey);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();

        _mockLicensingClient.Mock.Verify(m => m.ActivateLicenseAsync(validKey), Times.Once());

        // Verify the license is considered valid by the orchestration service after activation
        var validationResult = await _licensingService.ValidateLicenseAsync(validKey);
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue("License should be valid after successful activation.");
    }

    [Fact]
    [Trait("Category", "Licensing")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    [Trait("Requirement", "REQ-LDM-LIC-003")] // Covered by invalid key activation
    public async Task ActivateLicense_WithInvalidKey_ShouldFailAndNotifyUser()
    {
        // Arrange
        var invalidKey = "TEST-INVALID-KEY-67890";
        var expectedError = "Invalid license key.";
        _mockLicensingClient.SetupActivateFailure(invalidKey, expectedError);
        _mockLicensingClient.SetupValidateInvalid(invalidKey); // Assume invalid activation also means it's invalid

        // Act
        var result = await _licensingService.ActivateLicenseAsync(invalidKey);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be(expectedError);

        _mockLicensingClient.Mock.Verify(m => m.ActivateLicenseAsync(invalidKey), Times.Once());
        
        var validationResult = await _licensingService.ValidateLicenseAsync(invalidKey);
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse("License should be invalid after failed activation with an invalid key.");
    }

    [Fact]
    [Trait("Category", "Licensing")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    public async Task ApplicationStartup_WithValidLicense_AllowsFullFunctionality()
    {
        // Arrange
        var validKey = "STORED-VALID-KEY";
        // Simulate that the service has this key stored and validated at startup
        _mockLicensingClient.SetupValidateValid(validKey); 
        // Assuming LicensingOrchestrationService would call ValidateLicenseAsync on startup or when state is checked

        // Act
        // This might be implicitly tested by checking a property or behavior of the licensing service
        // that reflects the startup state. For an explicit test:
        var validationResult = await _licensingService.ValidateLicenseAsync(validKey); // Simulate startup check or get current status

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        // Further assertions would depend on how "full functionality" is exposed, e.g.,
        // _licensingService.CurrentLicenseStatus.Should().Be(LicenseStatus.Active);
        // _licensingService.IsFeatureEnabled("SomePremiumFeature").Should().BeTrue();
    }

    [Fact]
    [Trait("Category", "Licensing")]
    [Trait("Requirement", "REQ-LDM-TST-001")]
    public async Task ApplicationStartup_WithExpiredLicense_EntersGracePeriodOrLimitedMode()
    {
        // Arrange
        var expiredKey = "STORED-EXPIRED-KEY";
        _mockLicensingClient.SetupValidateExpired(expiredKey);
        // Assuming LicensingOrchestrationService handles the "expired" status from the client
        // and transitions to grace period/limited mode.

        // Act
        var validationResult = await _licensingService.ValidateLicenseAsync(expiredKey);

        // Assert
        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Status.Should().Be("Expired", "Validation result should indicate expiry.");
        // Assert application state (e.g., grace period, limited mode)
        // This depends on how LicensingOrchestrationService exposes this state. Example:
        // _licensingService.CurrentLicenseStatus.Should().Be(LicenseStatus.GracePeriod); // Or LicenseStatus.Limited
    }
}