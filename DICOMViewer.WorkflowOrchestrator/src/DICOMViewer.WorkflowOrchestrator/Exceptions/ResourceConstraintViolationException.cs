using System;

namespace TheSSS.DICOMViewer.Application.WorkflowOrchestrator.Exceptions
{
    public class ResourceConstraintViolationException : Exception
    {
        public ResourceConstraintViolationException(string message) : base(message) { }
        public ResourceConstraintViolationException(string message, Exception inner) : base(message, inner) { }
    }
}