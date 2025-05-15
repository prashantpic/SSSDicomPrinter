using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Common.DTOs;

public record DicomValidationResultDto
{
    public bool IsCompliant { get; init; }
    public List<string> ValidationErrors { get; init; } = new();
}