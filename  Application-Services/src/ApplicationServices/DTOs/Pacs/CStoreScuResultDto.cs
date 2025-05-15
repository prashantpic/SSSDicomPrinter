using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record CStoreScuResultDto(
        bool IsSuccessful,
        Dictionary<string, bool> InstanceSendStatuses,
        string ErrorMessage);
}