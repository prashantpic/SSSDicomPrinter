using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.DicomNetwork.DTOs
{
    public record PacsConfigurationDto(
        string PacsNodeId,
        string AETitle,
        string Hostname,
        int Port,
        string Description,
        bool IsDefault,
        List<string> SupportedTransferSyntaxes,
        DateTime? LastConnectionTestTime,
        string LastConnectionTestResult
    );
}