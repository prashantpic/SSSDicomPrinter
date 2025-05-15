using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for raising system alerts, 
/// used here to notify administrators of significant security events like license issues.
/// Implemented in Infrastructure or Cross-Cutting.
/// Requirements Addressed: REQ-LDM-LIC-005.
/// </summary>
public interface IAlertingService
{
    /// <summary>
    /// Raises an alert with the specified details.
    /// </summary>
    /// <param name="alertDetails">The details of the alert to raise.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RaiseAlertAsync(AlertDetailsDto alertDetails);
}