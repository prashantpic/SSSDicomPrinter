using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IAnonymizationServiceAdapter
    {
        Task AnonymizeDatasetAsync(string sopInstanceUid, int profileId);
        Task<string[]> GetAnonymizedInstancesAsync();
    }
}