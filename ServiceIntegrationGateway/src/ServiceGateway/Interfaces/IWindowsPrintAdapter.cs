using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for submitting print jobs to the Windows printing subsystem 
/// and reporting the outcome.
/// </summary>
public interface IWindowsPrintAdapter
{
    /// <summary>
    /// Prints a document using the Windows Print API.
    /// </summary>
    /// <param name="printJob">The print job DTO containing document and settings.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the result of the Windows print operation.</returns>
    Task<WindowsPrintResultDto> PrintDocumentAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);
}