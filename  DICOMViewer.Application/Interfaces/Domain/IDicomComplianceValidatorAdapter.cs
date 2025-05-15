using System.IO;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Common.DTOs;

namespace TheSSS.DICOMViewer.Application.Interfaces.Domain;

public interface IDicomComplianceValidatorAdapter
{
    Task<DicomValidationResultDto> ValidateComplianceAsync(Stream dicomStream);
}