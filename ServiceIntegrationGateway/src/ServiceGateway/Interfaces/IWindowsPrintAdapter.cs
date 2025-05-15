using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for submitting print jobs to the Windows printing subsystem and reporting the outcome.
/// This interface is for adapters interacting with the Windows Print API, abstracting native print operations.
/// </summary>
public interface IWindowsPrintAdapter
{
    /// <summary>
    /// Submits a print job to the Windows printing subsystem.
    /// </summary>
    /// <param name="printJob">The print job details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the print operation.</returns>
    Task<WindowsPrintResultDto> PrintDocumentAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);
}