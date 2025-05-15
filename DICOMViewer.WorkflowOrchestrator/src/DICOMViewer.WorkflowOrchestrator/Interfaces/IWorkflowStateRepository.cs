using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Interfaces
{
    public interface IWorkflowStateRepository
    {
        Task SaveStateAsync<TState>(string workflowId, TState state) where TState : class;
        Task<TState?> GetStateAsync<TState>(string workflowId) where TState : class;
        Task DeleteStateAsync(string workflowId);
    }
}