using System.Collections.Generic;
using System;

namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record CStoreScuResultDto(
        bool Success,
        string Message,
        int InstancesSent,
        int InstancesFailed,
        List<string> FailedInstanceUids,
        DateTime Timestamp);
}