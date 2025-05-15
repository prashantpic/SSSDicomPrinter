global using Xunit;
global using FluentAssertions;
global using Moq;
global using System;
global using System.Collections.Generic;
global using System.Diagnostics;
global using System.IO;
global using System.Linq;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging; // If ILogger is used directly in tests or fixtures
global using TheSSS.DicomViewer.Application.Services;
global using TheSSS.DicomViewer.Application.Services.Domain; // Assuming domain service interfaces might be here
global using TheSSS.DicomViewer.Domain.Entities;
global using TheSSS.DicomViewer.Domain.ValueObjects; // If any value objects are directly used
global using TheSSS.DicomViewer.Infrastructure.Data;
global using TheSSS.DicomViewer.Infrastructure.Repositories; // If repositories are directly referenced
global using TheSSS.DicomViewer.Presentation.ViewModels; // For UI interaction tests targeting ViewModels
global using TheSSS.DicomViewer.Common.Logging; // If a custom ILoggerAdapter is used
global using TheSSS.DicomViewer.IntegrationTests.Fixtures;
global using TheSSS.DicomViewer.IntegrationTests.Helpers;
global using TheSSS.DicomViewer.IntegrationTests.Mocks;

// Placeholder for actual types if not provided by referenced assemblies.
// These should ideally come from REPO-APP-SERVICES or REPO-INFRA.
// If these interfaces or DTOs (Data Transfer Objects) are defined in the actual
// referenced projects (REPO-APP-SERVICES, REPO-INFRA, REPO-DOMAIN),
// then these placeholder definitions are NOT needed and should be removed.

/*
namespace TheSSS.DicomViewer.Application.Services
{
    // Example DTO for License Activation
    public class LicenseActivationResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Example DTO for License Validation
    public class LicenseValidationResult
    {
        public bool IsValid { get; set; }
        public string? Status { get; set; } // e.g., "Active", "Expired", "GracePeriod"
        public DateTime? ExpiryDate { get; set; }
        public string? ErrorMessage { get; set; }
    }

    // Example Interface for external licensing API
    public interface ILicensingApiClient
    {
        Task<LicenseActivationResult> ActivateLicenseAsync(string licenseKey);
        Task<LicenseValidationResult> ValidateLicenseAsync(string licenseKey);
    }

    // Example Interface for SMTP service
    public interface ISmtpService
    {
        Task SendEmailAsync(string recipient, string subject, string body);
    }

    // Example Interface for Application Update Service
    public interface IApplicationUpdateService
    {
        Task<Version?> CheckForUpdatesAsync(); // Returns new version if available, null otherwise
        // bool IsUpdateAvailable { get; } // Could be a property
    }

    // Example Interface for Storage Monitor Service (used in AlertSystemTests)
    public interface IStorageMonitorService
    {
        Task<double> GetFreeSpacePercentageAsync(string path); // Returns free space percentage
    }

    // Example Interface for PACS Connectivity Monitor Service (used in AlertSystemTests)
    public class PacsNodeStatus { public string NodeName { get; set; } public bool IsConnected { get; set; } public string? ErrorMessage { get; set; } }
    public interface IPacsConnectivityMonitorService
    {
        Task<List<PacsNodeStatus>> CheckPacsNodesAsync();
    }

    // Example DTO for Search Criteria
    public class SearchCriteria
    {
        public string? PatientName { get; set; }
        public string? PatientId { get; set; }
        public DateTime? StudyDateFrom { get; set; }
        public DateTime? StudyDateTo { get; set; }
        public string? Modality { get; set; }
        // Add other relevant search fields
    }

    // Example DTO for Search Result
    public class StudySearchResult
    {
        public string StudyInstanceUid { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public string PatientId { get; set; } = string.Empty;
        public DateTime? StudyDate { get; set; }
        public string Modality { get; set; } = string.Empty;
        public string StudyDescription { get; set; } = string.Empty;
        public int NumberOfSeries { get; set; }
        public int NumberOfInstances { get; set; }
    }
}

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    // Example structure for a ViewModel used in rendering tests
    public class DicomImageViewerViewModel // : ObservableObject or your ViewModel base
    {
        public bool IsImageLoaded { get; private set; }
        // public ICommand LoadDicomFileCommand { get; } // Assume this exists

        public async Task LoadDicomFileCommandExecute(string filePath)
        {
            // Simulate loading logic
            await Task.Delay(100); // Simulate async work
            IsImageLoaded = !string.IsNullOrEmpty(filePath);
        }
        public void ResetView() { IsImageLoaded = false; }
        public Task WaitForFirstFrameRenderedAsync() => Task.CompletedTask; // Placeholder
    }
}
*/