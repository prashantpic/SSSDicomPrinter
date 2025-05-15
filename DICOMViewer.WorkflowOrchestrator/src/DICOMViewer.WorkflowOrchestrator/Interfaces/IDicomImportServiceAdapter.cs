using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IDicomImportServiceAdapter
    {
        Task ImportFilesAsync(string[] filePaths, object options);
        Task<string[]> GetDicomFilePathsAsync(string sourcePath);
    }
}