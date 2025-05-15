namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs
{
    public record AnonymizationRequestDto(
        string SopInstanceUid,
        string AnonymizationProfileId
    );
}