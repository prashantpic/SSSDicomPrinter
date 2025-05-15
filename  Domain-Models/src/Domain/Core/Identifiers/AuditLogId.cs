using TheSSS.DicomViewer.Domain.Exceptions;

namespace TheSSS.DicomViewer.Domain.Core.Identifiers
{
    public sealed record AuditLogId
    {
        public int Value { get; }

        public AuditLogId(int value)
        {
            if (value <= 0)
                throw new DomainException("Invalid Audit Log ID");
            
            Value = value;
        }
    }
}