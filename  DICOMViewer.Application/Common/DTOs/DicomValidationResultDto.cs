using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Common.DTOs
{
    public record DicomValidationResultDto(
        bool IsCompliant,
        List<string> ValidationErrors
    );
}