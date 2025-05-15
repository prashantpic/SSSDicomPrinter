namespace TheSSS.DicomViewer.Application.DTOs.Pacs
{
    public record PacsConfigurationDto(
        int Id,
        string AeTitle,
        string Host,
        int Port,
        string PeerAeTitle,
        bool IsDefault,
        bool IsActive,
        string Description);
}