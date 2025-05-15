// Assuming interfaces and return types are defined in TheSSS.DicomViewer.Application.Services
using TheSSS.DicomViewer.Application.Services; // For ILicensingApiClient, LicenseActivationResult, LicenseValidationResult
using Moq;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.IntegrationTests.Mocks
{
    public class MockOdooLicensingApiClient : ILicensingApiClient
    {
        private readonly Mock<ILicensingApiClient> _mock;

        public MockOdooLicensingApiClient()
        {
            _mock = new Mock<ILicensingApiClient>();
        }

        // Expose the internal mock for fine-grained setup in tests if needed
        public Mock<ILicensingApiClient> Mock => _mock;

        // Implement the interface by calling the internal mock
        public Task<LicenseActivationResult> ActivateLicenseAsync(string licenseKey)
        {
            return _mock.Object.ActivateLicenseAsync(licenseKey);
        }

        public Task<LicenseValidationResult> ValidateLicenseAsync(string licenseKey)
        {
            return _mock.Object.ValidateLicenseAsync(licenseKey);
        }

        // Helper methods for common mock setups
        public void SetupActivateSuccess(string licenseKey, string activatedProduct = "DICOM Viewer Pro")
        {
            _mock.Setup(m => m.ActivateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseActivationResult { IsSuccess = true, ErrorMessage = null, ActivatedProduct = activatedProduct });
        }

        public void SetupActivateFailure(string licenseKey, string errorMessage)
        {
            _mock.Setup(m => m.ActivateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseActivationResult { IsSuccess = false, ErrorMessage = errorMessage });
        }

        public void SetupValidateValid(string licenseKey, string status = "Active", string? expiryDate = null)
        {
            _mock.Setup(m => m.ValidateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseValidationResult 
                 { 
                     IsValid = true, 
                     Status = status, 
                     ExpiryDate = expiryDate ?? System.DateTime.UtcNow.AddYears(1).ToString("yyyy-MM-dd") 
                 });
        }

        public void SetupValidateInvalid(string licenseKey, string status = "Invalid Key")
        {
            _mock.Setup(m => m.ValidateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseValidationResult { IsValid = false, Status = status });
        }

        public void SetupValidateExpired(string licenseKey, string status = "Expired")
        {
            _mock.Setup(m => m.ValidateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseValidationResult 
                 { 
                     IsValid = false, 
                     Status = status, 
                     ExpiryDate = System.DateTime.UtcNow.Subtract(System.TimeSpan.FromDays(1)).ToString("yyyy-MM-dd") 
                 });
        }
        
        public void SetupValidateNearExpiry(string licenseKey, string status = "Near Expiry", int daysToExpiry = 10)
        {
            _mock.Setup(m => m.ValidateLicenseAsync(licenseKey))
                 .ReturnsAsync(new LicenseValidationResult 
                 { 
                     IsValid = true, // Still valid but near expiry
                     Status = status, 
                     ExpiryDate = System.DateTime.UtcNow.AddDays(daysToExpiry).ToString("yyyy-MM-dd") 
                 });
        }

        public void SetupThrowsExceptionOnActivation(string licenseKey, System.Exception exception)
        {
            _mock.Setup(m => m.ActivateLicenseAsync(licenseKey)).ThrowsAsync(exception);
        }

        public void SetupThrowsExceptionOnValidation(string licenseKey, System.Exception exception)
        {
            _mock.Setup(m => m.ValidateLicenseAsync(licenseKey)).ThrowsAsync(exception);
        }

        public void ResetMock()
        {
            _mock.Reset();
        }
    }
}