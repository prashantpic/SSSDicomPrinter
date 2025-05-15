using System;

namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record CEchoResultDto(
        bool IsSuccessful,
        TimeSpan ResponseTime,
        string ErrorMessage);
}