using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IResourceGovernor
    {
        Task<bool> TryAcquireResourcesAsync(string resourceType, int count);
        Task ReleaseResourcesAsync(string resourceType, int count);
        int CheckAvailableResources(string resourceType);
    }
}