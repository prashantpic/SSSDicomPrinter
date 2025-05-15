using MediatR;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Contracts.Commands
{
    public class StartDicomImportWorkflowCommand : IRequest
    {
        public required string SourcePath { get; init; }
        public required ImportOptions ImportOptions { get; init; }
    }

    public class ImportOptions
    {
        public bool OverwriteExisting { get; set; }
        public bool ValidateChecksums { get; set; }
        public int BatchSize { get; set; } = 100;
    }
}