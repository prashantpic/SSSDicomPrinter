using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for sending license validation requests to the Odoo API 
/// and processing its specific responses, including handling Odoo-specific error formats.
/// </summary>
public interface IOdooApiAdapter
{
    /// <summary>
    /// Validates a license with the Odoo API.
    /// </summary>
    /// <param name="request">The license request data.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the Odoo API response.</returns>
    Task<OdooLicenseResponseDto> ValidateLicenseAsync(OdooLicenseRequestDto request, CancellationToken cancellationToken = default);
}