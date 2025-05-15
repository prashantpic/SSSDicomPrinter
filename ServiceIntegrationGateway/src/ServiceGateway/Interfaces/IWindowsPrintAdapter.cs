using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming DTOs are in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for adapters interacting with the Windows Print API,
    /// abstracting native print operations.
    /// </summary>
    public interface IWindowsPrintAdapter
    {
        /// <summary>
        /// Submits a print job to the Windows printing subsystem.
        /// </summary>
        /// <param name="printJob">The print job details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The specific result from the Windows print operation.</returns>
        Task<WindowsPrintResultDto> PrintDocumentAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);
    }
}