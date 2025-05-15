using System;

namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record PacsConfigurationDto(
        int Id,
        string AeTitle,
        string Host,
        int Port,
        string Description,
        bool IsDefault,
        DateTimeOffset? LastSuccessfulConnection);
}