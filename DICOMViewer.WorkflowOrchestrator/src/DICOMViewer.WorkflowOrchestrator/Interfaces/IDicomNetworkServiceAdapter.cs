using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IDicomNetworkServiceAdapter
    {
        Task SendCEchoAsync(string aeTitle);
        Task SendCStoreAsync(string aeTitle, string[] dicomFiles);
    }
}