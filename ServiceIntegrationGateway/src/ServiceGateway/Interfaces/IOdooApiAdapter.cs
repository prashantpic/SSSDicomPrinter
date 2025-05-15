using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming DTOs are in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for adapters responsible for direct communication with the Odoo Licensing API,
    /// handling request formation and response parsing.
    /// </summary>
    public interface IOdooApiAdapter
    {
        /// <summary>
        /// Sends a license validation request to the Odoo API and processes the response.
        /// </summary>
        /// <param name="request">The Odoo license request DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The raw Odoo license response DTO.</returns>
        Task<OdooLicenseResponseDto> ValidateLicenseAsync(OdooLicenseRequestDto request, CancellationToken cancellationToken = default);
    }
}