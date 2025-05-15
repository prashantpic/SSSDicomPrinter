using System;

namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record CEchoResultDto(
        bool Success,
        string Message,
        TimeSpan ResponseTime,
        DateTime Timestamp);
}