using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for sending license validation requests to the Odoo API and processing its specific responses, 
/// including handling Odoo-specific error formats. This interface is for adapters responsible for direct communication 
/// with the Odoo Licensing API, handling request formation and response parsing.
/// </summary>
public interface IOdooApiAdapter
{
    /// <summary>
    /// Sends a license validation request to the Odoo API.
    /// </summary>
    /// <param name="request">The Odoo license request data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The raw Odoo API response data.</returns>
    Task<OdooLicenseResponseDto> ValidateLicenseAsync(OdooLicenseRequestDto request, CancellationToken cancellationToken = default);
}