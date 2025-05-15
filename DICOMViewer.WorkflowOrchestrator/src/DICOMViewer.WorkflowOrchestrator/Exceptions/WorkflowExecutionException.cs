using System;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions
{
    public class WorkflowExecutionException : Exception
    {
        public WorkflowExecutionException(string message) : base(message) { }
        public WorkflowExecutionException(string message, Exception inner) : base(message, inner) { }
    }
}