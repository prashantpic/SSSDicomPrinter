using System;
using TheSSS.DicomViewer.Domain.Validation;

namespace TheSSS.DicomViewer.Domain.Exceptions
{
    public class DicomComplianceFailedException : DomainException
    {
        public ComplianceReport Report { get; }

        public DicomComplianceFailedException(ComplianceReport report) : base("DICOM compliance validation failed")
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }

        public DicomComplianceFailedException(string message, ComplianceReport report) : base(message)
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }

        public DicomComplianceFailedException(string message, Exception inner, ComplianceReport report) : base(message, inner)
        {
            Report = report ?? throw new ArgumentNullException(nameof(report));
        }
    }
}