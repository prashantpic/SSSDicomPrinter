using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

public record PacsConfigurationDto
{
    public string PacsNodeId { get; init; } = default!;
    public string AETitle { get; init; } = default!;
    public string Hostname { get; init; } = default!;
    public int Port { get; init; }
    public string? Description { get; init; }
    public bool IsDefault { get; init; }
    public List<string> SupportedTransferSyntaxes { get; init; } = new();
    public DateTime? LastConnectionTestTime { get; init; }
    public string? LastConnectionTestResult { get; init; }
}