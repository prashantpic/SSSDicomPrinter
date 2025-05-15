namespace TheSSS.DicomViewer.Application.DTOs.Settings
{
    public record UserPreferenceDto(
        int UserId,
        bool EnableQuarantine,
        string QuarantinePath,
        bool EnableRejectedArchive,
        string RejectedArchivePath,
        string PreferredTheme,
        string PreferredLanguage);
}