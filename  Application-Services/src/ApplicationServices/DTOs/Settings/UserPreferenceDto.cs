namespace TheSSS.DicomViewer.Application.DTOs.Settings
{
    public record UserPreferenceDto(
        int UserId,
        string Theme,
        string Language,
        bool ShowAnnotations,
        int DefaultWindowLevelPresetId);
}